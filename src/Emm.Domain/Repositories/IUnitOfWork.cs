namespace Emm.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<string> GenerateNextCodeAsync<TEntity>(string prefix, int numberLength = 6, CancellationToken cancellationToken = default)
        where TEntity : class;

    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action);
}