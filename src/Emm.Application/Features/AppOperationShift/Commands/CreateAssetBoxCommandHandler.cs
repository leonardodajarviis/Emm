using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateAssetBoxCommandHandler : IRequestHandler<CreateAssetBoxCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQueryContext _queryContext;

    public CreateAssetBoxCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork,
        IQueryContext queryContext)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
        _queryContext = queryContext;
    }

    public async Task<Result<object>> Handle(CreateAssetBoxCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
            if (shift is null)
            {
                return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
            }

            // === PHASE 1: Create box and save to get ID ===
            shift.CreateAssetBox(
                request.BoxName,
                request.Role,
                request.DisplayOrder,
                request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Get the created box by name
            var createdBox = shift.AssetBoxes.FirstOrDefault(b => b.BoxName == request.BoxName);
            if (createdBox is null)
            {
                return Result<object>.Validation(
                    "Failed to create asset box",
                    ValidationErrorCodes.FieldRequired);
            }

            var existingAssetIds = new List<Guid>();
            var newAssetIds = new List<Guid>();
            var assignedAssetIds = new List<Guid>();

            // === PHASE 2: Add/assign assets with Box ID ===
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
                    return Result<object>.Validation(
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
                        // Asset already in shift, just assign to box
                        existingAssetIds.Add(asset.Id);
                        shift.AssignAssetToBox(asset.Id, createdBox.Id);
                        assignedAssetIds.Add(asset.Id);
                    }
                    else
                    {
                        // Asset not in shift yet, add it and assign to box
                        newAssetIds.Add(asset.Id);
                        shift.AddAsset(
                            assetId: asset.Id,
                            assetCode: asset.Code,
                            assetName: asset.DisplayName,
                            isPrimary: false,
                            assetBoxId: createdBox.Id);
                        assignedAssetIds.Add(asset.Id);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                createdBox.Id,
            });
        });
    }
}
