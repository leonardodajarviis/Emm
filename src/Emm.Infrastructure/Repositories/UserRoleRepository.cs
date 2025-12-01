using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly XDbContext _context;

    public UserRoleRepository(XDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserRole>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId && ur.Role.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(long roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .Include(ur => ur.User)
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> GetAsync(long userId, long roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task AddAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        await _context.Set<UserRole>().AddAsync(userRole, cancellationToken);
    }

    public Task DeleteAsync(UserRole userRole, CancellationToken cancellationToken = default)
    {
        _context.Set<UserRole>().Remove(userRole);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(long userId, long roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserRole>()
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }
}
