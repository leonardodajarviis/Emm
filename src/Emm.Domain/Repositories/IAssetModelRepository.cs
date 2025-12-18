using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Domain.Repositories;

public interface IAssetModelRepository : IRepository<AssetModel, Guid>
{
    Task<AssetModel?> GetByCodeAsync(string code);

    Task<bool> ExistsByCodeAsync(string code);
}
