using Emm.Infrastructure.Data;

using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class AssetModelRepository : GenericRepository<AssetModel, long>, IAssetModelRepository
{
    public AssetModelRepository(XDbContext context) : base(context)
    {
    }

    public override Task<AssetModel?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(am => am.Parameters)
            .Include(am => am.Images)
            .Include(am => am.MaintenancePlanDefinitions)
            .ThenInclude(am => am.JobSteps)
            .Include(am => am.MaintenancePlanDefinitions)
            .ThenInclude(am => am.ParameterBasedTrigger);

        return query.FirstOrDefaultAsync(am => am.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await DbSet.AnyAsync(am => am.Code == code);
    }

    public async Task<AssetModel?> GetByCodeAsync(string code)
    {
        return await DbSet
            .Include(am => am.Parameters)
            .Include(am => am.Images)
            .Include(am => am.MaintenancePlanDefinitions)
            .ThenInclude(am => am.JobSteps)
            .Include(am => am.MaintenancePlanDefinitions)
            .ThenInclude(am => am.ParameterBasedTrigger)
            .FirstOrDefaultAsync(am => am.Code == code);
    }
}
