using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Emm.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateOperationShiftCommandHandler : IRequestHandler<CreateOperationShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IQueryContext _queryContext;
    private readonly IUserContextService _userContextService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IDateTimeProvider _clock;
    private readonly IMediator _mediator;

    public CreateOperationShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IQueryContext queryContext,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService,
        IMediator mediator,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _queryContext = queryContext;
        _userContextService = userContextService;
        _codeGenerator = codeGenerator;
        _clock = dateTimeProvider;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateOperationShiftCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _codeGenerator.GenerateNextCodeAsync<OperationShift>("NKVH", 8, cancellationToken);

            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                return Result.Internal("Không thể xác định người dùng hiện tại");
            }

            var user = await _queryContext.Query<User>()
                .FirstOrDefaultAsync(u => u.Id == userId.Value, cancellationToken);

            if (user == null)
            {
                return Result.Unauthorized("Thông tin người dùng không tồn tại");
            }

            var organizationUnitId = user.OrganizationUnitId;
            if (organizationUnitId == null)
            {
                return Result.Invalid("Người dùng không thuộc đơn vị tổ chức nào");
            }


            var assetIds = request.Assets.Select(a => a.AssetId).ToList();

            var existingAssets = await _queryContext.Query<Asset>()
                .Include(a => a.Parameters)
                .Where(a => assetIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, cancellationToken);

            if (existingAssets.Any(a => a.Value.Status != AssetStatus.Idle))
            {
                return Result.Validation("Có một tài sản đang không ở trạng thái rảnh rỗi", ShiftLogErrorCodes.AssetNotInIdleStatus);
            }


            // Create operation shift
            var operationShift = new OperationShift(
                code: code,
                name: request.Name,
                primaryUserId: userId.Value,
                organizationUnitId: organizationUnitId.Value,
                scheduledStartTime: _clock.Now,
                scheduledEndTime: _clock.Now.AddHours(24),
                description: request.Description,
                notes: request.Notes
            );

            foreach (var assetRequest in request.Assets)
            {
                existingAssets.TryGetValue(assetRequest.AssetId, out var asset);

                if (asset == null)
                {
                    return Result.Invalid($"Không tìm thấy tài sản với ID {assetRequest.AssetId}");
                }

                operationShift.AddAsset( asset.Id, asset.Code.Value, asset.DisplayName);
            }

            foreach (var parameters in existingAssets.Values.Select(a => a.Parameters))
            {
                foreach (var parameter in parameters)
                {
                    operationShift.AddReadingSnapshot(parameter.AssetId, parameter.ParameterId, parameter.CurrentValue);
                }
            }

            if (request.AssetBoxes.Count > 0)
            {
                foreach (var boxRequest in request.AssetBoxes)
                {
                    operationShift.CreateAssetBox(
                        boxName: boxRequest.BoxName,
                        role: boxRequest.Role,
                        assetIds: boxRequest.AssetIds,
                        displayOrder: boxRequest.DisplayOrder,
                        description: boxRequest.Description
                    );
                }
            }


            operationShift.StartShift(_clock.Now);
            await _repository.AddAsync(operationShift, cancellationToken);
            foreach (var evt in operationShift.ImmediateEvents)
            {
                await _mediator.Publish(evt, cancellationToken);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new
            {
                operationShift.Id,
            });
        });
    }
}
