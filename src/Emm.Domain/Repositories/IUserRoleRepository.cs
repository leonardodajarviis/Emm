using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IUserRoleRepository
{
    Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<UserRole?> GetAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserRole userRole, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
