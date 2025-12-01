using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;

namespace Emm.Infrastructure.Repositories;

public class AssetTypeRepository : GenericRepository<AssetType, long>, IAssetTypeRepository
{
    public AssetTypeRepository(XDbContext context) : base(context)
    {
    }
}