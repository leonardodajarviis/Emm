using Emm.Domain.Abstractions;

namespace Emm.Domain.Repositories;

public interface IRepository<TEntity, TKey> where TEntity : class, IAggregateRoot
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetForUpdateAsync(TKey id, CancellationToken cancellationToken = default); // nếu cần tách
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    void AddRange(IReadOnlyCollection<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
