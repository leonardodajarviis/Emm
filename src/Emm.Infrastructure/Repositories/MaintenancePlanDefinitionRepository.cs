using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class MaintenancePlanDefinitionRepository : GenericRepository<MaintenancePlanDefinition, Guid>, IMaintenancePlanDefinitionRepository
{
    public MaintenancePlanDefinitionRepository(XDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MaintenancePlanDefinition>> GetByAssetModelIdAsync(Guid assetModelId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(mpd => mpd.AssetModelId == assetModelId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAndAssetModelIdAsync(string name, Guid assetModelId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(mpd => mpd.Name == name && mpd.AssetModelId == assetModelId, cancellationToken);
    }
}
