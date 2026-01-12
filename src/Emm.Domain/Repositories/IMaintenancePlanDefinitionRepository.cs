using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Domain.Repositories;

public interface IMaintenancePlanDefinitionRepository : IRepository<MaintenancePlanDefinition, Guid>
{
    Task<IEnumerable<MaintenancePlanDefinition>> GetByAssetModelIdAsync(Guid assetModelId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAndAssetModelIdAsync(string name, Guid assetModelId, CancellationToken cancellationToken = default);
}
