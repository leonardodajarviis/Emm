using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class AssetRepository : GenericRepository<Asset, Guid>, IAssetRepository
{
    public AssetRepository(XDbContext context) : base(context)
    {
    }

    public async Task IdleMultiAsync(IEnumerable<Guid> assetIds, CancellationToken cancellationToken = default)
    {
        var assets = await DbSet.Where(a => assetIds.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken);
        foreach (var asset in assets)
        {
            asset.Idle();
        }
    }


    public async Task OperateMultiAsync(IEnumerable<Guid> assetIds, CancellationToken cancellationToken = default)
    {
        var assets = await DbSet.Where(a => assetIds.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken);
        foreach (var asset in assets)
        {
            asset.Operate();
        }
    }

    public async Task<List<Asset>> GetMultiByIdsAsync(IEnumerable<Guid> assetIds, CancellationToken cancellationToken = default)
    {
        var assets = await DbSet
            .Include(a => a.ParameterMaintenances)
            .Include(a => a.Parameters)
            .Where(a => assetIds.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken);
        return assets;
    }
}
