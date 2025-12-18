using Emm.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Common;

public interface IForeignKeyValidator
{
    Task<Result> ValidateAsync<TEntity>(Guid id, string? entityName = null, CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<Result> ValidateAsync<TEntity>(Guid? id, string? entityName = null, CancellationToken cancellationToken = default)
        where TEntity : class;

    Task<Result> ValidateManyAsync<TEntity>(IEnumerable<Guid> ids, string? entityName = null, CancellationToken cancellationToken = default)
        where TEntity : class;
}

public class ForeignKeyValidator : IForeignKeyValidator
{
    private readonly IQueryContext _queryContext;

    public ForeignKeyValidator(IQueryContext queryContext)
    {
        _queryContext = queryContext;
    }

    public async Task<Result> ValidateAsync<TEntity>(Guid id, string? entityName = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var name = entityName ?? typeof(TEntity).Name;
        var exists = await _queryContext.Query<TEntity>()
            .Where(e => EF.Property<Guid>(e, "Id") == id)
            .AnyAsync(cancellationToken);

        return exists
            ? Result.Success()
            : Result.NotFound($"{name} with ID {id} not found", $"{name.ToUpperInvariant()}_NOT_FOUND");
    }

    public async Task<Result> ValidateAsync<TEntity>(Guid? id, string? entityName = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (id == null)
        {
            return Result.Success();
        }

        return await ValidateAsync<TEntity>(id.Value, entityName, cancellationToken);
    }

    public async Task<Result> ValidateManyAsync<TEntity>(IEnumerable<Guid> ids, string? entityName = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var idList = ids.ToList();
        if (!idList.Any())
        {
            return Result.Success();
        }

        var name = entityName ?? typeof(TEntity).Name;
        var existingIds = await _queryContext.Query<TEntity>()
            .Where(e => idList.Contains(EF.Property<Guid>(e, "Id")))
            .Select(e => EF.Property<Guid>(e, "Id"))
            .ToListAsync(cancellationToken);

        var missingIds = idList.Except(existingIds).ToList();

        return missingIds.Any()
            ? Result.NotFound(
                $"{name}(s) not found: {string.Join(", ", missingIds)}",
                $"{name.ToUpperInvariant()}_NOT_FOUND")
            : Result.Success();
    }
}
