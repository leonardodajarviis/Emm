using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role, long>, IRoleRepository
{
    public RoleRepository(XDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByIdWithPermissionsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(r => r.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (!includeInactive)
            query = query.Where(r => r.IsActive);

        return await query
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role)
            .Where(r => r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public void Delete(Role role)
    {
        Remove(role);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(r => r.Code == code, cancellationToken);
    }
}
