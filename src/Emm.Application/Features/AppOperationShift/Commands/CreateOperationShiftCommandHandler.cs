using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Emm.Domain.Entities.Operations.CreationData;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateOperationShiftCommandHandler : IRequestHandler<CreateOperationShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IQueryContext _queryContext;
    private readonly IUserContextService _userContextService;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
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
        _dateTimeProvider = dateTimeProvider;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateOperationShiftCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _codeGenerator.GenerateNextCodeAsync<OperationShift>("NKVH", 8, cancellationToken);

            var organizationUnitId = _userContextService.GetCurrentOrganizationUnitId();
            if (organizationUnitId == null)
            {
                return Result.Validation(
                    "Organization unit is required",
                    ValidationErrorCodes.FieldRequired);
            }

            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
            {
                return Result.Unauthorized(
                    "User information not found",
                    AuthErrorCodes.SessionInvalid);
            }

            var assignAssets = new List<AssignAssetData>();

            var assetIds = request.Assets.Select(a => a.AssetId).ToList();

            var existingAssets = await _queryContext.Query<Asset>()
                .Include(a => a.Parameters)
                .Where(a => assetIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, cancellationToken);

            foreach (var assetRequest in request.Assets)
            {
                existingAssets.TryGetValue(assetRequest.AssetId, out var asset);

                if (asset == null)
                {
                    return Result.Invalid($"Asset with ID {assetRequest.AssetId} does not exist.");
                }

                assignAssets.Add(new AssignAssetData
                {
                    AssetId = asset.Id,
                    AssetCode = asset.Code.Value,
                    AssetName = asset.DisplayName,
                    IsPrimary = assetRequest.IsPrimary
                });
            }

            // Create operation shift
            var operationShift = new OperationShift(
                code: code,
                name: request.Name,
                primaryUserId: userId.Value,
                organizationUnitId: organizationUnitId.Value,
                scheduledStartTime: _dateTimeProvider.Now,
                scheduledEndTime: _dateTimeProvider.Now.AddHours(24),
                assets: assignAssets,
                description: request.Description,
                notes: request.Notes
            );

            foreach(var parameters in existingAssets.Values.Select(a => a.Parameters))
            {
                foreach(var parameter in parameters)
                {
                    operationShift.AddReadingSnapshot(
                        parameter.AssetId,
                        parameter.ParameterId,
                        parameter.CurrentValue);
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


            operationShift.StartShift(_dateTimeProvider.Now);
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
