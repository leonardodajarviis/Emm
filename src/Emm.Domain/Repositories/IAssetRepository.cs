using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Domain.Repositories;

public interface IAssetRepository : IRepository<Asset, long>
{
    Task OperateMultiAsync(IEnumerable<long> assetIds, CancellationToken cancellationToken = default);
    Task IdleMultiAsync(IEnumerable<long> assetIds, CancellationToken cancellationToken = default);
}
