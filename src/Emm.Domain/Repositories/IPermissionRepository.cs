using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default);
    Task AddAsync(Permission permission, CancellationToken cancellationToken = default);
    void Update(Permission permission);
    void Delete(Permission permission);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}
