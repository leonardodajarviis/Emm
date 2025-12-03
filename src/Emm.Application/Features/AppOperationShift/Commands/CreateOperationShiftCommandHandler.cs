using Emm.Application.Abstractions;
using Emm.Application.Common.ErrorCodes;
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
    private readonly IOutbox _outbox;
    private readonly ICodeGenerator _codeGenerator;

    public CreateOperationShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IQueryContext queryContext,
        ICodeGenerator codeGenerator,
        IUserContextService userContextService,

        IOutbox outbox)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _qq = queryContext;
        _userContextService = userContextService;
        _outbox = outbox;
        _codeGenerator = codeGenerator;
    }

    public async Task<Result<object>> Handle(CreateOperationShiftCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var code = await _codeGenerator.GenerateNextCodeAsync("NKVH", "OperationShifts", 6, cancellationToken);

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
                primaryEmployeeId: userId.Value,
                organizationUnitId: organizationUnitId.Value,
                scheduledStartTime: DateTime.UtcNow,
                scheduledEndTime: DateTime.UtcNow.AddHours(12),
                notes: request.Notes
            );

            // Step 1: Create asset groups first (if provided)
            // This creates groups with their LinkedId so assets can reference them
            var groupLinkedIdToIdMap = new Dictionary<Guid, long>();
            if (request.AssetGroups?.Count > 0)
            {
                foreach (var groupRequest in request.AssetGroups)
                {
                    var groupId = operationShift.CreateAssetGroup(
                        linkedId: groupRequest.LinkedId,
                        groupName: groupRequest.GroupName,
                        role: groupRequest.Role,
                        displayOrder: groupRequest.DisplayOrder,
                        description: groupRequest.Description
                    );
                    groupLinkedIdToIdMap[groupRequest.LinkedId] = groupId;
                }
            }

            // Step 2: Validate and add assets
            if (request.Assets?.Count > 0)
            {
                // Validate all GroupLinkedIds exist in the groups we just created
                var groupLinkedIds = request.Assets
                    .Where(a => a.GroupLinkedId.HasValue)
                    .Select(a => a.GroupLinkedId!.Value)
                    .Distinct()
                    .ToList();

                if (groupLinkedIds.Count > 0)
                {
                    operationShift.ValidateGroupLinkedIds(groupLinkedIds);
                }

                // Get all asset IDs to fetch from database
                var assetIds = request.Assets.Select(a => a.AssetId).Distinct().ToList();
                var assets = await _qq.Query<Asset>()
                    .Where(a => assetIds.Contains(a.Id))
                    .ToListAsync(cancellationToken);

                // Validate all requested assets exist
                var foundAssetIds = assets.Select(a => a.Id).ToHashSet();
                var missingAssetIds = assetIds.Where(id => !foundAssetIds.Contains(id)).ToList();
                if (missingAssetIds.Count > 0)
                {
                    return Result<object>.Validation(
                        $"Assets not found: {string.Join(", ", missingAssetIds)}",
                        ValidationErrorCodes.FieldRequired);
                }

                // Add assets to the shift
                foreach (var assetRequest in request.Assets)
                {
                    var asset = assets.First(a => a.Id == assetRequest.AssetId);

                    // Resolve group ID from LinkedId if provided
                    long? assetGroupId = null;
                    if (assetRequest.GroupLinkedId.HasValue)
                    {
                        assetGroupId = operationShift.GetGroupIdByLinkedId(assetRequest.GroupLinkedId.Value);
                    }

                    operationShift.AddAsset(
                        assetId: asset.Id,
                        assetCode: asset.Code,
                        assetName: asset.DisplayName,
                        isPrimary: assetRequest.IsPrimary,
                        assetGroupId: assetGroupId
                    );
                }
            }

            // Start the shift
            operationShift.StartShift(DateTime.UtcNow);

            await _repository.AddAsync(operationShift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                operationShift.Id,
                operationShift.Code,
                AssetGroups = operationShift.AssetGroups.Select(g => new
                {
                    g.Id,
                    g.LinkedId,
                    g.GroupName,
                    g.Role
                }),
                Assets = operationShift.Assets.Select(a => new
                {
                    a.AssetId,
                    a.AssetCode,
                    a.AssetName,
                    a.AssetGroupId,
                    a.IsPrimary
                })
            });
        });
    }
}
