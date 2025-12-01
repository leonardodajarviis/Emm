namespace Emm.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<string> GenerateNextCodeAsync(string prefix, string tableName, int numberLength = 6, CancellationToken cancellationToken = default);

    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action);
}