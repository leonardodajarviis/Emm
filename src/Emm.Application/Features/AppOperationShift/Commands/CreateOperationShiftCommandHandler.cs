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

            // === PHASE 1: Create Boxes and save to get IDs ===
            var boxNameToAssetIdsMap = new Dictionary<string, List<Guid>>();

            if (request.AssetBoxes?.Count > 0)
            {
                foreach (var boxRequest in request.AssetBoxes)
                {
                    operationShift.CreateAssetBox(
                        boxName: boxRequest.BoxName,
                        role: boxRequest.Role,
                        displayOrder: boxRequest.DisplayOrder,
                        description: boxRequest.Description
                    );

                    // Store mapping for later
                    if (boxRequest.AssetIds?.Any() == true)
                    {
                        boxNameToAssetIdsMap[boxRequest.BoxName] = boxRequest.AssetIds.ToList();
                    }
                }
            }

            // Save to get Box IDs
            await _repository.AddAsync(operationShift, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // === PHASE 2: Add Assets with Box IDs ===
            // Collect all asset IDs from both direct assets and box assets
            var allAssetIds = new HashSet<Guid>();
            if (request.Assets?.Count > 0)
            {
                foreach (var asset in request.Assets)
                    allAssetIds.Add(asset.AssetId);
            }
            foreach (var assetIds in boxNameToAssetIdsMap.Values)
            {
                foreach (var assetId in assetIds)
                    allAssetIds.Add(assetId);
            }

            if (allAssetIds.Count > 0)
            {
                // Fetch all assets from database
                var assets = await _qq.Query<Asset>()
                    .Where(a => allAssetIds.Contains(a.Id))
                    .ToListAsync(cancellationToken);

                // Validate all requested assets exist
                var foundAssetIds = assets.Select(a => a.Id).ToHashSet();
                var missingAssetIds = allAssetIds.Where(id => !foundAssetIds.Contains(id)).ToList();
                if (missingAssetIds.Count > 0)
                {
                    return Result<object>.Validation(
                        $"Assets not found: {string.Join(", ", missingAssetIds)}",
                        ValidationErrorCodes.FieldRequired);
                }

                // Add direct assets (without box)
                if (request.Assets?.Count > 0)
                {
                    foreach (var assetRequest in request.Assets)
                    {
                        var asset = assets.First(a => a.Id == assetRequest.AssetId);
                        operationShift.AddAsset(
                            assetId: asset.Id,
                            assetCode: asset.Code.Value,
                            assetName: asset.DisplayName,
                            isPrimary: assetRequest.IsPrimary,
                            assetBoxId: null
                        );
                    }
                }

                // Add assets to boxes
                foreach (var (boxName, assetIds) in boxNameToAssetIdsMap)
                {
                    var box = operationShift.AssetBoxes.FirstOrDefault(b => b.BoxName == boxName);
                    if (box == null) continue;

                    foreach (var assetId in assetIds)
                    {
                        var asset = assets.First(a => a.Id == assetId);
                        operationShift.AddAsset(
                            assetId: asset.Id,
                            assetCode: asset.Code.Value,
                            assetName: asset.DisplayName,
                            isPrimary: false,
                            assetBoxId: box.Id
                        );
                    }
                }
            }

            // Start the shift
            operationShift.StartShift(DateTime.UtcNow);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object>.Success(new
            {
                operationShift.Id,
            });
        });
    }
}
