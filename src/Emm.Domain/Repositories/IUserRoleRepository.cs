using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IUserRoleRepository
{
    Task<IReadOnlyList<UserRole>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(long roleId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetAsync(long userId, long roleId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long userId, long roleId, CancellationToken cancellationToken = default);
}
