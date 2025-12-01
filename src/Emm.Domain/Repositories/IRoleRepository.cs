using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdWithPermissionsAsync(long id, CancellationToken cancellationToken = default);
    Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task AddAsync(Role role, CancellationToken cancellationToken = default);
    void Update(Role role);
    void Delete(Role role);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}
