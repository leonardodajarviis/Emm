using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Events.AssetAddition;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class AssetAdditionCreatedEventHandler : IEventHandler<AssetAdditionCreatedEvent>
{
    private readonly IQueryContext _queryContext;
    private readonly IRepository<Asset, long> _assetRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssetAdditionCreatedEventHandler(
        IQueryContext queryContext,
        IRepository<Asset, long> assetRepository,
        IUnitOfWork unitOfWork)
    {
        _queryContext = queryContext;
        _assetRepository = assetRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AssetAdditionCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);

        var assetModelIds = @event.AssetLines
            .Select(line => line.AssetModelId)
            .Distinct()
            .ToList();

        var assetModels = await _queryContext.Query<AssetModel>()
            .Where(x => assetModelIds.Contains(x.Id))
            .Include(x => x.Parameters)
            .ToDictionaryAsync(x => x.Id, cancellationToken: cancellationToken);

        var assets = new List<Asset>();

        foreach (var line in @event.AssetLines)
        {
            if (!assetModels.TryGetValue(line.AssetModelId, out var assetModel))
            {
                continue; // Skip if the asset model is not found
            }

            var asset = new Asset(
                code: line.AssetCode,
                displayName: assetModel.Name,
                assetModelId: line.AssetModelId,
                assetCategoryId: assetModel.AssetCategoryId,
                assetTypeId: assetModel.AssetTypeId,
                organizationUnitId: @event.OrganizationUnitId,
                locationId: @event.LocationId,
                assetAdditionId: @event.AssetAdditionId,
                description: assetModel.Description);

            // Add parameters from the asset model
            if (assetModel.Parameters.Count > 0)
            {
                asset.AddParameters([.. assetModel.Parameters.Select(p => p.ParameterId)]);
            }

            assets.Add(asset);
        }

        // Only add to repository - SaveChanges will be called by the outbox processor
        _assetRepository.AddRange(assets);
    }
}
