using Emm.Domain.DomainEvents.AssetAdditionEvents;
using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class AssetAdditionCreatedEventHandler : IEventHandler<AssetAdditionCreatedEvent>
{
    private readonly IQueryContext _qq;
    private readonly IRepository<Asset, long> _assetRepository;
    private readonly IUnitOfWork _uow;

    public AssetAdditionCreatedEventHandler(
        IQueryContext queryContext,
        IRepository<Asset, long> assetRepository,
        IUnitOfWork uow)
    {
        _qq = queryContext;
        _assetRepository = assetRepository;
        _uow = uow;
    }

    public async Task Handle(AssetAdditionCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var assetModelIds = @event.AssetLines
            .Select(line => line.AssetModelId)
            .Distinct()
            .ToList();

        var assetModels = await _qq.Query<AssetModel>()
            .Where(x => assetModelIds.Contains(x.Id))
            .Include(x => x.Parameters)
            .Include(x => x.MaintenancePlanDefinitions)
            .ThenInclude(x => x.JobSteps)
            .ToDictionaryAsync(x => x.Id, cancellationToken: cancellationToken);


        var assets = new List<Asset>();
        // foreach (var line in @event.AssetLines)
        // {
        //     if (!assetModels.TryGetValue(line.AssetModelId, out var assetModel))
        //     {
        //         continue; // Skip if the asset model is not found
        //     }

        //     var asset = new Asset(
        //         code: line.AssetCode,
        //         displayName: assetModel.Name,
        //         assetModelId: line.AssetModelId,
        //         organizationUnitId: @event.OrganizationUnitId,
        //         locationId: @event.LocationId,
        //         assetAdditionId: @event.AssetAdditionId,
        //         description: assetModel.Description); // Description can be set later if needed

        //     asset.AddParameters([..assetModel.Parameters.Select(p => p.ParameterId)]);


        //     foreach (var maintenancePlanDefinition in assetModel.MaintenancePlanDefinitions)
        //     {
        //         asset.AddMaintenancePlanInstance(
        //             isActive: maintenancePlanDefinition.IsActive,
        //             parameterId: maintenancePlanDefinition.ParameterId,
        //             value: maintenancePlanDefinition.Value,
        //             min: maintenancePlanDefinition.Min,
        //             max: maintenancePlanDefinition.Max,
        //             jobSteps: [.. maintenancePlanDefinition.JobSteps.Select(step => new MaintenancePlanJobStepInstanceSpec(
        //                 Name: step.Name,
        //                 OrganizationUnitId: step.OrganizationUnitId,
        //                 Note: step.Note,
        //                 Order: step.Order
        //             ))]);
        //     }

        //     assets.Add(asset);
        // }
        _assetRepository.AddRange(assets);
        await _uow.SaveChangesAsync(cancellationToken);

        return;
    }
}
