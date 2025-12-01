using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IUserPermissionRepository
{
    Task<IReadOnlyList<UserPermission>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<UserPermission?> GetAsync(long userId, long permissionId, CancellationToken cancellationToken = default);
    Task AddAsync(UserPermission userPermission, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserPermission userPermission, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long userId, long permissionId, CancellationToken cancellationToken = default);
}
