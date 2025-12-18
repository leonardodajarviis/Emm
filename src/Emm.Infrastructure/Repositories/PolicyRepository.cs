using Emm.Domain.Entities.Authorization;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class PolicyRepository : GenericRepository<Policy, Guid>, IPolicyRepository
{
    public PolicyRepository(XDbContext context) : base(context)
    {
    }

    public async Task<Policy?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(p => p.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Policy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsQueryable();

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query
            .OrderByDescending(p => p.Priority)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Policy>> GetByResourceTypeAsync(string resourceType, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive && (p.ResourceType == resourceType || p.ResourceType == null))
            .OrderByDescending(p => p.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Policy>> GetActivePoliciesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.Priority)
            .ToListAsync(cancellationToken);
    }

    public void Delete(Policy policy)
    {
        Remove(policy);
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(p => p.Code == code, cancellationToken);
    }
}
