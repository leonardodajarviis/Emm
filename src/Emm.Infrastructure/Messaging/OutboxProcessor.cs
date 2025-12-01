using Emm.Infrastructure.Data;
using Emm.Infrastructure.Messaging;
using LazyNet.Symphony.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        // _log.LogInformation("OutboxProcessor started with LockId={LockId}", _lockId);

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
                // app shutting down
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Outbox loop crashed");
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

        // 1) Claim batch (đặt lock để tránh đụng nhau)
        var candidates = await db.OutboxMessages
            .Where(m => m.ProcessedAt == null &&
                        (m.LockedUntil == null || m.LockedUntil < now))
            .OrderBy(m => m.CreatedAt)
            .Take(_opt.BatchSize)
            .ToListAsync(ct);

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
            // bị tranh chấp lock — fine, lát nữa lọc lại của mình.
        }

        // 2) Lấy hàng mình đã lock
        var mine = await db.OutboxMessages
            .Where(m => m.ProcessedAt == null && m.LockId == _lockId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync(ct);

        if (mine.Count == 0) return 0;

        int ok = 0;
        foreach (var msg in mine)
        {
            try
            {
                // 3) Deserialize → Publish
                var evt = serializer.Deserialize(msg.Type, msg.Payload);

                // Nếu bạn dùng MediatR, IEvent nên : INotification
                // và publish như dưới:
                await mediator.Publish((object)evt, ct);

                // 4) Đánh dấu done
                msg.ProcessedAt = DateTime.UtcNow;
                msg.Error = null;
                ok++;
            }
            catch (Exception ex)
            {
                // 5) Retry / DLQ
                msg.Attempt += 1;
                msg.Error = ex.ToString();

                if (msg.Attempt >= _opt.MaxAttempts)
                {
                    // dead-letter: mark processed để không đụng nữa (hoặc chuyển bảng khác tuỳ bạn)
                    msg.ProcessedAt = DateTime.UtcNow;
                    _log.LogError(ex, "Outbox message {Id} moved to dead-letter after {Attempt} attempts", msg.Id, msg.Attempt);
                }
                else
                {
                    // gia hạn để lần sau xử lý lại (backoff)
                    msg.LockedUntil = DateTime.UtcNow + _opt.GetBackoff(msg.Attempt);
                }
            }
            finally
            {
                // thả lock (đã xử lý hoặc đã gia hạn)
                msg.LockId = null;
            }
        }

        await db.SaveChangesAsync(ct);
        return ok;
    }
}
