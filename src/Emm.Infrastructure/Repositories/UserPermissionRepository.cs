using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly XDbContext _context;

    public UserPermissionRepository(XDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserPermission>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserPermission>()
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserPermission?> GetAsync(long userId, long permissionId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserPermission>()
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, cancellationToken);
    }

    public async Task AddAsync(UserPermission userPermission, CancellationToken cancellationToken = default)
    {
        await _context.Set<UserPermission>().AddAsync(userPermission, cancellationToken);
    }

    public Task DeleteAsync(UserPermission userPermission, CancellationToken cancellationToken = default)
    {
        _context.Set<UserPermission>().Remove(userPermission);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(long userId, long permissionId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserPermission>()
            .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId, cancellationToken);
    }
}
