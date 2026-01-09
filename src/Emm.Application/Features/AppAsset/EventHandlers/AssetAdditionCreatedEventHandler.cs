using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Events.AssetAddition;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class AssetAdditionCreatedEventHandler : IEventHandler<AssetAdditionCreatedEvent>
{
    private readonly IQueryContext _queryContext;
    private readonly IAssetRepository _assetRepository;

    public AssetAdditionCreatedEventHandler(
        IQueryContext queryContext,
        IAssetRepository assetRepository)
    {
        _queryContext = queryContext;
        _assetRepository = assetRepository;
    }

    public async Task Handle(AssetAdditionCreatedEvent @event, CancellationToken cancellationToken)
    {
        var assetModelIds = @event.AssetLines
            .Select(line => line.AssetModelId)
            .Distinct()
            .ToList();

        var assetModels = await _queryContext.Query<AssetModel>()
            .Where(x => assetModelIds.Contains(x.Id))
            .Include(x => x.Parameters)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var categoryIds = assetModels.Values
            .Select(am => am.AssetCategoryId)
            .Distinct()
            .ToList();

        var typeIds = assetModels.Values
            .Select(am => am.AssetTypeId)
            .Distinct()
            .ToList();

        var parameterIds = assetModels.Values
            .SelectMany(am => am.Parameters)
            .Select(p => p.ParameterId)
            .Distinct()
            .ToList();

        var assetCategories = await _queryContext.Query<AssetCategory>()
            .Where(ac => categoryIds.Contains(ac.Id))
            .ToDictionaryAsync(ac => ac.Id, cancellationToken);

        var assetTypes = await _queryContext.Query<AssetType>()
            .Where(at => typeIds.Contains(at.Id))
            .ToDictionaryAsync(at => at.Id, cancellationToken);

        var parameters = await _queryContext.Query<ParameterCatalog>()
            .Where(p => parameterIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);


        var assets = new List<Asset>();

        foreach (var line in @event.AssetLines)
        {
            if (!assetModels.TryGetValue(line.AssetModelId, out var assetModel))
            {
                continue; // Skip if the asset model is not found
            }

            if (!assetCategories.TryGetValue(assetModel.AssetCategoryId, out var assetCategory))
            {
                continue; // Skip if the asset category is not found
            }

            if (!assetTypes.TryGetValue(assetModel.AssetTypeId, out var assetType))
            {
                continue; // Skip if the asset type is not found
            }

            var asset = new Asset(
                isCodeGenerated: line.IsCodeGenerated,
                code: line.AssetCode,
                displayName: line.AssetDisplayName,
                assetModelId: line.AssetModelId,
                assetCategoryId: assetModel.AssetCategoryId,
                assetTypeId: assetModel.AssetTypeId,
                organizationUnitId: @event.OrganizationUnitId,
                locationId: @event.LocationId,
                assetAdditionId: @event.AssetAdditionId,
                description: assetModel.Description);

            asset.MakeSnapshot(
                assetModeCode: assetModel.Code.Value,
                assetModelName: assetModel.Name,
                assetTypeCode: assetType.Code.Value,
                assetTypeName: assetType.Name,
                assetCategoryCode: assetCategory.Code.Value,
                assetCategoryName: assetCategory.Name);

            // Add parameters from the asset model
            if (assetModel.Parameters.Count > 0)
            {
                foreach (var assetModelParameter in assetModel.Parameters)
                {
                    if (!parameters.TryGetValue(assetModelParameter.ParameterId, out var parameterCatalog))
                    {
                        continue; // Skip if the parameter catalog is not found
                    }
                    asset.AddParameter(
                        parameterId: assetModelParameter.ParameterId,
                        isMaintenanceParameter: assetModelParameter.IsMaintenanceParameter,
                        parameterCode: parameterCatalog.Code,
                        parameterName: parameterCatalog.Name,
                        unitOfMeasureId: parameterCatalog.UnitOfMeasureId,
                        value: 0
                    );
                }
            }

            var maintenancePlans = await _queryContext.Query<MaintenancePlanDefinition>()
                .Include(mp => mp.ParameterBasedTrigger)
                .Where(mp => mp.AssetModelId == assetModel.Id && mp.PlanType == MaintenancePlanType.ParameterBased)
                .ToListAsync(cancellationToken);

            foreach (var plan in maintenancePlans)
            {
                var timeBaseMaintenance = plan.ParameterBasedTrigger;
                if (timeBaseMaintenance == null) continue;

                asset.AddParameterMaintenance(
                    parameterId: timeBaseMaintenance.ParameterId,
                    maintenancePlanId: plan.Id,
                    thresholdValue: timeBaseMaintenance.Value,
                    plusTolerance: timeBaseMaintenance.PlusTolerance,
                    minusTolerance: timeBaseMaintenance.MinusTolerance
                );
            }

            assets.Add(asset);
        }

        // Add to repository and let interceptor save after all events processed
        _assetRepository.AddRange(assets);
    }
}
