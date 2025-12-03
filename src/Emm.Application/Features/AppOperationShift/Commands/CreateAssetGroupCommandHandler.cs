using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateAssetGroupCommandHandler : IRequestHandler<CreateAssetGroupCommand, Result<CreateAssetGroupResult>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQueryContext _queryContext;

    public CreateAssetGroupCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork,
        IQueryContext queryContext)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
        _queryContext = queryContext;
    }

    public async Task<Result<CreateAssetGroupResult>> Handle(CreateAssetGroupCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<CreateAssetGroupResult>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        // Generate a new LinkedId for this group
        var linkedId = Guid.NewGuid();

        // Step 1: Create the asset group
        var groupId = shift.CreateAssetGroup(
            linkedId,
            request.GroupName,
            request.Role,
            request.DisplayOrder,
            request.Description);

        var existingAssetIds = new List<long>();
        var newAssetIds = new List<long>();
        var assignedAssetIds = new List<long>();

        // Step 2: Process assets if provided
        if (request.AssetIds?.Count > 0)
        {
            // Get asset details from database
            var assets = await _queryContext.Query<Asset>()
                .Where(a => request.AssetIds.Contains(a.Id))
                .ToListAsync(cancellationToken);

            // Validate all assets exist
            var foundAssetIds = assets.Select(a => a.Id).ToHashSet();
            var missingAssetIds = request.AssetIds.Where(id => !foundAssetIds.Contains(id)).ToList();
            if (missingAssetIds.Count > 0)
            {
                return Result<CreateAssetGroupResult>.Validation(
                    $"Assets not found: {string.Join(", ", missingAssetIds)}",
                    ValidationErrorCodes.FieldRequired);
            }

            // Get existing assets in the shift
            var existingShiftAssetIds = shift.Assets.Select(a => a.AssetId).ToHashSet();

            foreach (var asset in assets)
            {
                // Check if asset already exists in shift
                if (existingShiftAssetIds.Contains(asset.Id))
                {
                    // Asset already in shift, just assign to group
                    existingAssetIds.Add(asset.Id);
                    shift.AssignAssetToGroup(asset.Id, groupId);
                    assignedAssetIds.Add(asset.Id);
                }
                else
                {
                    // Asset not in shift yet, add it and assign to group
                    newAssetIds.Add(asset.Id);
                    shift.AddAsset(
                        assetId: asset.Id,
                        assetCode: asset.Code,
                        assetName: asset.DisplayName,
                        isPrimary: false,
                        assetGroupId: groupId);
                    assignedAssetIds.Add(asset.Id);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CreateAssetGroupResult>.Success(new CreateAssetGroupResult
        {
            GroupId = groupId,
            LinkedId = linkedId,
            AssignedAssetIds = assignedAssetIds,
            ExistingAssetIds = existingAssetIds,
            NewAssetIds = newAssetIds
        });
    }
}
