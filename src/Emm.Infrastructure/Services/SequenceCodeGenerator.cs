using Emm.Domain.Repositories;
using Emm.Domain.ValueObjects;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace Emm.Infrastructure.Services;

public class SequenceCodeGenerator : ICodeGenerator, IDisposable
{
    private readonly XDbContext _dbContext;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
    private static readonly ConcurrentDictionary<string, DateTime> _semaphoreLastUsed = new();
    private static readonly SemaphoreSlim _cleanupSemaphore = new(1, 1);
    private static readonly TimeSpan _semaphoreTimeout = TimeSpan.FromMinutes(30);
    private static DateTime _lastCleanup = DateTime.UtcNow;

    public SequenceCodeGenerator(XDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<NaturalKey> GetNaturalKeyAsync(
        string prefix,
        string tableName,
        int numberLength = 6,
        CancellationToken cancellationToken = default)
    {
        var code = await GenerateNextCodeAsync(prefix, tableName, numberLength, cancellationToken);

        var numberPart = code[prefix.Length..];
        if (!int.TryParse(numberPart, out var number))
        {
            throw new InvalidOperationException("Generated code has invalid format");
        }

        return NaturalKey.Create(prefix, number, numberLength);
    }

    public async Task<string> GenerateNextCodeAsync(
        string prefix,
        string tableName,
        int numberLength = 6,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Prefix cannot be null or empty", nameof(prefix));

        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));

        if (numberLength < 1 || numberLength > 20)
            throw new ArgumentException("Number length must be between 1 and 20", nameof(numberLength));

        var semaphoreKey = $"{tableName}_{prefix}_{numberLength}";
        var semaphore = _semaphores.GetOrAdd(semaphoreKey, _ => new SemaphoreSlim(1, 1));
        _semaphoreLastUsed[semaphoreKey] = DateTime.UtcNow;

        await CleanupOldSemaphoresAsync();

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            IDbContextTransaction? transaction = null;
            var createdLocalTx = false;

            if (_dbContext.Database.CurrentTransaction == null)
            {
                transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
                createdLocalTx = true;
            }

            try
            {
                var sequenceNumber = await _dbContext.SequenceNumbers
                    .Where(s => s.Prefix == prefix && s.TableName == tableName && s.NumberLength == numberLength)
                    .FirstOrDefaultAsync(cancellationToken);

                long nextNumber;

                if (sequenceNumber == null)
                {
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
                    nextNumber = sequenceNumber.CurrentNumber + 1;

                    var maxNumber = (long)Math.Pow(10, numberLength) - 1;
                    if (nextNumber > maxNumber)
                    {
                        throw new InvalidOperationException(
                            $"Sequence number overflow: next number {nextNumber} exceeds maximum {maxNumber} for length {numberLength}");
                    }

                    sequenceNumber.CurrentNumber = nextNumber;
                    sequenceNumber.UpdatedAt = DateTime.UtcNow;
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

                if (createdLocalTx && transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                return $"{prefix}{nextNumber.ToString().PadLeft(numberLength, '0')}";
            }
            catch
            {
                if (createdLocalTx && transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                throw;
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static async Task CleanupOldSemaphoresAsync()
    {
        if (DateTime.UtcNow - _lastCleanup < TimeSpan.FromMinutes(10))
            return;

        if (!_cleanupSemaphore.Wait(0))
            return;

        try
        {
            var now = DateTime.UtcNow;
            var keysToRemove = _semaphoreLastUsed
                .Where(kvp => now - kvp.Value > _semaphoreTimeout)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in keysToRemove)
            {
                if (_semaphores.TryRemove(key, out var semaphore))
                {
                    semaphore.Dispose();
                    _semaphoreLastUsed.TryRemove(key, out _);
                }
            }

            _lastCleanup = now;
        }
        finally
        {
            _cleanupSemaphore.Release();
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
