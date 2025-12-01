using Emm.Domain.Abstractions;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IAggregateRoot
{
    protected readonly XDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(XDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = Context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public Task<TEntity?> GetForUpdateAsync(TKey id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }


    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual void AddRange(IReadOnlyCollection<TEntity> entities)
    {
        ArgumentNullException.ThrowIfNull(entities);
        DbSet.AddRange(entities);
    }

    public void Add(TEntity entity)
    {
        DbSet.Add(entity);
    }


    public void Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Remove(entity);
    }
}
