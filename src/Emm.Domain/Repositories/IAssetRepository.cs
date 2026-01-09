using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Domain.Repositories;

public interface IAssetRepository : IRepository<Asset, Guid>
{
    Task OperateMultiAsync(IEnumerable<Guid> assetIds, CancellationToken cancellationToken = default);
    Task IdleMultiAsync(IEnumerable<Guid> assetIds, CancellationToken cancellationToken = default);
    Task<List<Asset>> GetMultiByIdsAsync(IEnumerable<Guid> assetIds, CancellationToken cancellationToken = default);
}
