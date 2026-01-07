using System.Data;
using Emm.Infrastructure.Data;
using LazyNet.Symphony.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Emm.Infrastructure.Messaging;

/// <summary>
/// Cấu hình cho OutboxProcessor
/// </summary>
public sealed class OutboxProcessorOptions
{
    /// <summary>Số lượng messages tối đa trong mỗi batch</summary>
    public int BatchSize { get; set; } = 50;

    /// <summary>Số lần thử lại tối đa trước khi chuyển vào dead-letter</summary>
    public int MaxAttempts { get; set; } = 10;

    /// <summary>Số lượng messages xử lý đồng thời tối đa</summary>
    public int MaxConcurrency { get; set; } = 10;

    /// <summary>Thời gian giữ lock trên message khi đang xử lý</summary>
    public TimeSpan LockDuration { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>Thời gian chờ khi không có message nào để xử lý</summary>
    public TimeSpan IdleDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Tính toán thời gian backoff theo exponential backoff strategy
    /// </summary>
    public TimeSpan GetBackoff(int attempt)
    {
        // Exponential backoff: 2^(attempt-1) seconds, tối đa 60 giây
        var seconds = Math.Min(60, Math.Pow(2, Math.Max(0, attempt - 1)));
        return TimeSpan.FromSeconds(seconds);
    }
}

/// <summary>
/// Trạng thái xử lý của message
/// </summary>
internal enum MessageProcessingStatus
{
    Success,            // Xử lý thành công
    Failed,             // Xử lý thất bại, cần retry
    MovedToDeadLetter   // Vượt quá số lần retry, chuyển vào dead-letter
}

/// <summary>
/// Kết quả xử lý một message
/// </summary>
internal sealed class MessageProcessingResult
{
    public MessageProcessingStatus Status { get; init; }
    public Exception? Exception { get; init; }
    public TimeSpan? RetryBackoff { get; init; }

    public static MessageProcessingResult Success() => new() { Status = MessageProcessingStatus.Success };

    public static MessageProcessingResult Failed(Exception ex, TimeSpan backoff) =>
        new() { Status = MessageProcessingStatus.Failed, Exception = ex, RetryBackoff = backoff };

    public static MessageProcessingResult DeadLetter(Exception ex) =>
        new() { Status = MessageProcessingStatus.MovedToDeadLetter, Exception = ex };
}


/// <summary>
/// Background service xử lý Outbox Pattern để đảm bảo eventual consistency
/// Sử dụng database-level locking (UPDLOCK + READPAST) để tránh race condition
/// </summary>
public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxProcessorOptions _opt;
    private readonly ILogger<OutboxProcessor> _logger;

    // LockId duy nhất cho mỗi instance để track messages đang được xử lý
    private readonly string _lockId = $"host-{Environment.MachineName}-{Guid.NewGuid():N}";

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        IOptions<OutboxProcessorOptions> options,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _opt = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessor started with LockId={LockId}", _lockId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessBatch(stoppingToken);
                if (processed == 0)
                {
                    await Task.Delay(_opt.IdleDelay, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("OutboxProcessor is shutting down gracefully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OutboxProcessor batch execution. Waiting before retry");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("OutboxProcessor stopped");
    }

    private async Task<int> ProcessBatch(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<XDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var serializer = scope.ServiceProvider.GetRequiredService<IEventSerializer>();

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

        try
        {
            var now = DateTime.UtcNow;

            // ============================================================
            // BƯỚC 1: Claim messages atomically bằng database-level locking
            // ============================================================
            // UPDLOCK: Ngăn các processor khác đọc các rows này để update
            //          → Tránh race condition khi nhiều processors chạy đồng thời
            // READPAST: Bỏ qua các rows đang bị lock
            //           → Tránh blocking, mỗi processor lấy messages khác nhau
            // Kết hợp 2 hints này đảm bảo:
            //   - Không có 2 processors nào claim cùng 1 message
            //   - Processors không block lẫn nhau
            var sql = @"
                SELECT TOP(@batchSize) *
                FROM OutboxMessages WITH (UPDLOCK, READPAST)
                WHERE ProcessedAt IS NULL
                  AND (LockedUntil IS NULL OR LockedUntil < @now)
                ORDER BY CreatedAt";

            var candidates = await db.OutboxMessages
                .FromSqlRaw(sql,
                    new Microsoft.Data.SqlClient.SqlParameter("@batchSize", _opt.BatchSize),
                    new Microsoft.Data.SqlClient.SqlParameter("@now", now))
                .ToListAsync(ct);

            if (candidates.Count == 0)
            {
                await transaction.CommitAsync(ct);
                return 0;
            }

            _logger.LogDebug("Claimed {Count} messages for processing", candidates.Count);

            // Set lock ngay lập tức trong transaction để đánh dấu ownership
            // LockId: Định danh processor đang xử lý message này
            // LockedUntil: Thời gian hết hạn lock (failsafe nếu processor crash)
            foreach (var m in candidates)
            {
                m.LockId = _lockId;
                m.LockedUntil = now.Add(_opt.LockDuration);
            }

            await db.SaveChangesAsync(ct);

            // ============================================================
            // BƯỚC 2: Xử lý messages song song với controlled concurrency
            // ============================================================
            // Sử dụng SemaphoreSlim để giới hạn số tasks chạy đồng thời
            // → Tránh overload system khi có quá nhiều messages
            // → Balance giữa throughput và resource usage
            var semaphore = new SemaphoreSlim(_opt.MaxConcurrency);
            var processingTasks = candidates.Select(async msg =>
            {
                await semaphore.WaitAsync(ct);  // Chờ slot available
                try
                {
                    var result = await ProcessSingleMessage(msg, mediator, serializer, ct);
                    return (Message: msg, Result: result);
                }
                finally
                {
                    semaphore.Release();  // Release slot cho task khác
                }
            }).ToArray();

            // Đợi tất cả tasks hoàn thành trước khi apply results
            var results = await Task.WhenAll(processingTasks);

            // ============================================================
            // BƯỚC 3: Apply kết quả xử lý vào messages
            // ============================================================
            // Update trạng thái dựa trên kết quả xử lý:
            //   - Success: Đánh dấu ProcessedAt, clear lock
            //   - Failed: Tăng Attempt, schedule retry với backoff
            //   - DeadLetter: Đánh dấu ProcessedAt để không retry nữa
            int successCount = 0;
            int failureCount = 0;
            int deadLetterCount = 0;

            foreach (var (msg, result) in results)
            {
                switch (result.Status)
                {
                    case MessageProcessingStatus.Success:
                        msg.ProcessedAt = DateTime.UtcNow;  // Đánh dấu đã xử lý
                        msg.Error = null;
                        msg.LockId = null;  // Release lock
                        successCount++;

                        _logger.LogDebug("Successfully processed message {MessageId} of type {MessageType}",
                            msg.Id, msg.Type);
                        break;

                    case MessageProcessingStatus.Failed:
                        msg.Attempt += 1;  // Tăng số lần thử
                        msg.Error = result.Exception?.ToString();
                        // Schedule retry với exponential backoff
                        msg.LockedUntil = DateTime.UtcNow + result.RetryBackoff!.Value;
                        msg.LockId = null;  // Release lock để có thể retry
                        failureCount++;

                        _logger.LogDebug(
                            "Message {MessageId} scheduled for retry in {Backoff}. Attempt {Attempt}/{MaxAttempts}",
                            msg.Id, result.RetryBackoff, msg.Attempt, _opt.MaxAttempts);
                        break;

                    case MessageProcessingStatus.MovedToDeadLetter:
                        msg.Attempt += 1;
                        msg.Error = result.Exception?.ToString();
                        // Đánh dấu ProcessedAt để dừng retry cycle
                        // Message này sẽ không được pick up nữa, cần manual intervention
                        msg.ProcessedAt = DateTime.UtcNow;
                        msg.LockId = null;
                        deadLetterCount++;

                        _logger.LogWarning(
                            "Message {MessageId} moved to dead-letter after {Attempts} failed attempts",
                            msg.Id, msg.Attempt);
                        break;
                }
            }

            // ============================================================
            // BƯỚC 4: Commit tất cả thay đổi trong 1 transaction
            // ============================================================
            // Atomic commit đảm bảo:
            //   - Hoặc tất cả messages được update thành công
            //   - Hoặc không có gì thay đổi (nếu có lỗi)
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            if (successCount > 0 || failureCount > 0 || deadLetterCount > 0)
            {
                _logger.LogInformation(
                    "Batch processed: {Success} succeeded, {Failed} failed, {DeadLetter} dead-lettered out of {Total} messages",
                    successCount, failureCount, deadLetterCount, candidates.Count);
            }

            return successCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during batch processing, rolling back transaction");

            try
            {
                await transaction.RollbackAsync(ct);
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "Error during transaction rollback");
            }

            throw;
        }
    }

    /// <summary>
    /// Xử lý một message đơn lẻ
    /// Method này chỉ chứa business logic, không touch database
    /// </summary>
    private async Task<MessageProcessingResult> ProcessSingleMessage(
        OutboxMessage message,
        IMediator mediator,
        IEventSerializer serializer,
        CancellationToken ct)
    {
        try
        {
            // Validate dữ liệu message
            if (string.IsNullOrWhiteSpace(message.Type) || string.IsNullOrWhiteSpace(message.Payload))
            {
                throw new InvalidOperationException(
                    $"Invalid message data: Type={message.Type}, PayloadLength={message.Payload?.Length ?? 0}");
            }

            // Deserialize JSON payload thành event object
            var evt = serializer.Deserialize(message.Type, message.Payload);

            if (evt == null)
            {
                throw new InvalidOperationException($"Deserialization returned null for type {message.Type}");
            }

            // Publish event qua MediatR để các handlers xử lý
            await mediator.Publish((object)evt, ct);

            return MessageProcessingResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process message {MessageId} of type {MessageType}. Attempt {Attempt}/{MaxAttempts}",
                message.Id, message.Type, message.Attempt + 1, _opt.MaxAttempts);

            // Kiểm tra xem đã vượt quá số lần retry chưa
            if (message.Attempt + 1 >= _opt.MaxAttempts)
            {
                // Chuyển vào dead-letter queue
                return MessageProcessingResult.DeadLetter(ex);
            }

            // Schedule retry với exponential backoff
            // Attempt 1: 1s, Attempt 2: 2s, Attempt 3: 4s, ..., max 60s
            var backoff = _opt.GetBackoff(message.Attempt + 1);
            return MessageProcessingResult.Failed(ex, backoff);
        }
    }
}
