using Emm.Domain.Entities.Authorization;

namespace Emm.Domain.Repositories;

public interface IPolicyRepository
{
    Task<Policy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Policy?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Policy>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Policy>> GetByResourceTypeAsync(string resourceType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Policy>> GetActivePoliciesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Policy policy, CancellationToken cancellationToken = default);
    void Update(Policy policy);
    void Delete(Policy policy);
    Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);
}
