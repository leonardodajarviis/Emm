using Emm.Infrastructure.Data;
using LazyNet.Symphony.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emm.Infrastructure.Messaging;

public sealed class OutboxProcessorOptions
{
    public int BatchSize { get; set; } = 50;
    public int MaxAttempts { get; set; } = 10;
    public TimeSpan LockDuration { get; set; } = TimeSpan.FromMinutes(1);
    public TimeSpan IdleDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    public TimeSpan GetBackoff(int attempt)
    {
        var seconds = Math.Min(60, Math.Pow(2, Math.Max(0, attempt - 1)));
        return TimeSpan.FromSeconds(seconds);
    }
}


public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _log;
    private readonly OutboxProcessorOptions _opt;
    private readonly string _lockId = $"host-{Environment.MachineName}-{Guid.NewGuid():N}";

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxProcessorOptions> options,
        ILogger<OutboxProcessor> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
        _opt = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("OutboxProcessor started with LockId={LockId}", _lockId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessBatch(stoppingToken);
                if (processed == 0)
                    await Task.Delay(_opt.IdleDelay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Application is shutting down
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox processing loop encountered an error");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }

        _log.LogInformation("OutboxProcessor stopped");
    }

    private async Task<int> ProcessBatch(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var serializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

        var now = DateTime.UtcNow;

        // 1) Claim batch: acquire lock on unprocessed messages
        var candidates = await db.OutboxMessages
            .Where(m => m.ProcessedAt == null &&
                        (m.LockedUntil == null || m.LockedUntil < now))
            .OrderBy(m => m.CreatedAt)
            .Take(_opt.BatchSize)
            .ToListAsync(ct);

        if (candidates.Count == 0) return 0;

        foreach (var m in candidates)
        {
            m.LockId = _lockId;
            m.LockedUntil = now.Add(_opt.LockDuration);
        }

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Lock contention occurred - another processor claimed some messages
            // Re-query to get only the messages we successfully locked
            db.ChangeTracker.Clear();

            var mine = await db.OutboxMessages
                .Where(m => m.ProcessedAt == null && m.LockId == _lockId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync(ct);

            candidates = mine;
        }

        if (candidates.Count == 0) return 0;

        // 2) Process each locked message
        int ok = 0;
        foreach (var msg in candidates)
        {
            try
            {
                // Validate message type before deserialization
                if (string.IsNullOrWhiteSpace(msg.Type) || string.IsNullOrWhiteSpace(msg.Payload))
                {
                    throw new InvalidOperationException($"Invalid message data: Type={msg.Type}, PayloadLength={msg.Payload?.Length ?? 0}");
                }

                // 3) Deserialize and publish event
                var evt = serializer.Deserialize(msg.Type, msg.Payload);

                if (evt == null)
                {
                    throw new InvalidOperationException($"Deserialization returned null for type {msg.Type}");
                }

                await mediator.Publish((object)evt, ct);

                // 4) Mark as processed
                msg.ProcessedAt = DateTime.UtcNow;
                msg.Error = null;
                ok++;
            }
            catch (Exception ex)
            {
                // 5) Handle retry logic and dead-letter queue
                msg.Attempt += 1;
                msg.Error = ex.ToString();

                if (msg.Attempt >= _opt.MaxAttempts)
                {
                    // Move to dead-letter: mark as processed to stop retrying
                    msg.ProcessedAt = DateTime.UtcNow;
                    _log.LogError(ex, "Outbox message {Id} moved to dead-letter after {Attempt} attempts", msg.Id, msg.Attempt);
                }
                else
                {
                    // Schedule retry with exponential backoff
                    msg.LockedUntil = DateTime.UtcNow + _opt.GetBackoff(msg.Attempt);
                    _log.LogWarning(ex, "Outbox message {Id} failed attempt {Attempt}, will retry after {RetryAfter}",
                        msg.Id, msg.Attempt, msg.LockedUntil);
                }
            }
            finally
            {
                // Release lock (either processed or rescheduled)
                msg.LockId = null;
            }
        }

        await db.SaveChangesAsync(ct);
        return ok;
    }
}
