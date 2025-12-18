using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IUserPermissionRepository
{
    Task<IReadOnlyList<UserPermission>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserPermission?> GetAsync(Guid userId, Guid permissionId, CancellationToken cancellationToken = default);
    Task AddAsync(UserPermission userPermission, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserPermission userPermission, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid permissionId, CancellationToken cancellationToken = default);
}
