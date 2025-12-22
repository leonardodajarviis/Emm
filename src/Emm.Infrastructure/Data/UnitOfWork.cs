using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace Emm.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly XDbContext _dbContext;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    public UnitOfWork(XDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> action)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<string> GenerateNextCodeAsync<TEntity>(string prefix, int numberLength = 6, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

        if (numberLength < 1 || numberLength > 20)
            throw new ArgumentException("Number length must be between 1 and 20", nameof(numberLength));

        // Get table name from Entity Framework metadata
        var tableName = _dbContext.Model.FindEntityType(typeof(TEntity))?.GetTableName()
            ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not configured in DbContext");

        // Create a unique key for the semaphore based on prefix and table name
        var semaphoreKey = $"{tableName}_{prefix}_{numberLength}";
        var semaphore = _semaphores.GetOrAdd(semaphoreKey, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            // Use ambient transaction if exists; otherwise create a short one
            var createdLocalTx = false;
            if (_dbContext.Database.CurrentTransaction == null)
            {
                await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                createdLocalTx = true;
            }

            try
            {
                // Find existing sequence record with row-level locking (SQL Server syntax)
                var sequenceNumber = await _dbContext.SequenceNumbers
                    .FromSqlRaw("SELECT * FROM SequenceNumbers WITH (UPDLOCK, HOLDLOCK, ROWLOCK) WHERE Prefix = {0} AND TableName = {1} AND NumberLength = {2}", prefix, tableName, numberLength)
                    .FirstOrDefaultAsync(cancellationToken);

                long nextNumber;

                if (sequenceNumber == null)
                {
                    // Create new sequence record
                    nextNumber = 1;
                    sequenceNumber = new SequenceNumber
                    {
                        Prefix = prefix,
                        TableName = tableName,
                        NumberLength = numberLength,
                        CurrentNumber = nextNumber,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _dbContext.SequenceNumbers.Add(sequenceNumber);
                }
                else
                {
                    // Increment existing sequence
                    nextNumber = sequenceNumber.CurrentNumber + 1;
                    sequenceNumber.CurrentNumber = nextNumber;
                    sequenceNumber.UpdatedAt = DateTime.UtcNow;
                }

                // Persist the reservation of this number within the current transaction
                await _dbContext.SaveChangesAsync(cancellationToken);

                if (createdLocalTx)
                {
                    await _dbContext.Database.CommitTransactionAsync(cancellationToken);
                }

                // Format the code with the specified number length
                var numberPart = nextNumber.ToString().PadLeft(numberLength, '0');
                return $"{prefix}{numberPart}";
            }
            catch
            {
                if (createdLocalTx)
                {
                    await _dbContext.Database.RollbackTransactionAsync(cancellationToken);
                }
                throw;
            }
        }
        finally
        {
            semaphore.Release();
        }
    }
}
