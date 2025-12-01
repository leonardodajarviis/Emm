using Emm.Domain.Entities.AssetCatalog;
using Emm.Infrastructure.Data;

namespace Emm.Infrastructure.Repositories;

public class AssetRepository : GenericRepository<Asset, Guid>
{
    public AssetRepository(XDbContext context) : base(context)
    {
    }
}
