using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class AssetTypeRepository : GenericRepository<AssetType, long>, IAssetTypeRepository
{
    public AssetTypeRepository(XDbContext context) : base(context)
    {

    }

    public override async Task<AssetType?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(at => at.Parameters)
            .FirstOrDefaultAsync(at => at.Id == id);
    }
}
