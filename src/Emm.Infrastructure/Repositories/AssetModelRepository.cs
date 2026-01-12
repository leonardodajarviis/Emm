using Emm.Infrastructure.Data;

using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class AssetModelRepository : GenericRepository<AssetModel, Guid>, IAssetModelRepository
{
    public AssetModelRepository(XDbContext context) : base(context)
    {
    }

    public override Task<AssetModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(am => am.Parameters)
            .Include(am => am.Images);

        return query.FirstOrDefaultAsync(am => am.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await DbSet.AnyAsync(am => am.Code.Value == code);
    }

    public async Task<AssetModel?> GetByCodeAsync(string code)
    {
        return await DbSet
            .Include(am => am.Parameters)
            .Include(am => am.Images)
            .FirstOrDefaultAsync(am => am.Code.Value == code);
    }
}
