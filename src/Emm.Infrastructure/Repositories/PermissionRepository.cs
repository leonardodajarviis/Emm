using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class PermissionRepository : GenericRepository<Permission, long>, IPermissionRepository
{
    public PermissionRepository(XDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByResourceAsync(string resource, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Resource == resource)
            .OrderBy(p => p.Action)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.Category == category)
            .OrderBy(p => p.Resource)
            .ThenBy(p => p.Action)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken = default)
    {
        var codeList = codes.ToList();
        return await DbSet
            .Where(p => codeList.Contains(p.Code))
            .ToListAsync(cancellationToken);
    }

    public void Delete(Permission permission)
    {
        Remove(permission);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(p => p.Code == code, cancellationToken);
    }
}
