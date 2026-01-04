using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateOperationShiftCommandHandler : IRequestHandler<CreateOperationShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IQueryContext _qq;
    private readonly IUserContextService _userContextService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateOperationShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IQueryContext queryContext,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _qq = queryContext;
        _userContextService = userContextService;
        _codeGenerator = codeGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<object>> Handle(CreateOperationShiftCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _codeGenerator.GenerateNextCodeAsync<OperationShift>("NKVH", 6, cancellationToken);

            var organizationUnitId = _userContextService.GetCurrentOrganizationUnitId();
            if (organizationUnitId == null)
            {
                return Result<object>.Validation(
                    "Organization unit is required",
                    ValidationErrorCodes.FieldRequired);
            }

            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                return Result<object>.Unauthorized(
                    "User information not found",
                    AuthErrorCodes.SessionInvalid);
            }

            // Create operation shift
            var operationShift = new OperationShift(
                code: code,
                name: request.Name,
                primaryUserId: userId.Value,
                organizationUnitId: organizationUnitId.Value,
                scheduledStartTime: _dateTimeProvider.Now,
                scheduledEndTime: _dateTimeProvider.Now.AddHours(12),
                notes: request.Notes
            );

            if (request.Assets?.Count > 0)
            {
                var assetIds = request.Assets.Select(a => a.AssetId).ToList();
                var existingAssets = await _qq.Query<Asset>()
                    .Where(a => assetIds.Contains(a.Id))
                    .ToDictionaryAsync(a => a.Id, cancellationToken);

                foreach (var assetRequest in request.Assets)
                {
                    existingAssets.TryGetValue(assetRequest.AssetId, out var asset);

                    if (asset == null)
                    {
                        return Result<object>.Invalid($"Asset with ID {assetRequest.AssetId} does not exist.");
                    }

                    operationShift.AddAsset(
                        assetId: asset.Id,
                        assetCode: asset.Code.Value,
                        assetName: asset.DisplayName
                    );
                }
            }

            if (request.AssetBoxes?.Count > 0)
            {
                foreach (var boxRequest in request.AssetBoxes)
                {
                    operationShift.CreateAssetBox(
                        boxName: boxRequest.BoxName,
                        role: boxRequest.Role,
                        assetIds: boxRequest.AssetIds
                        displayOrder: boxRequest.DisplayOrder,
                        description: boxRequest.Description,
                    );
                }
            }


            operationShift.StartShift(DateTime.UtcNow);
            await _repository.AddAsync(operationShift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                operationShift.Id,
            });
        });
    }
}
