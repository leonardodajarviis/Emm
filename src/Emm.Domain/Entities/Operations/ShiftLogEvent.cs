using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Lịch sử trạng thái và sự kiện của Task (nghỉ ca, sự cố, ghi chú)
/// </summary>
public class ShiftLogEvent
{
    public long Id { get; private set; }
    public long ShiftLogId { get; private set; }
    public ShiftLogEventType EventType { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public TimeSpan? Duration { get; private set; }

    public ShiftLogEvent(
        long operationTaskId,
        ShiftLogEventType eventType,
        DateTime startTime
        )
    {
        if (operationTaskId <= 0)
            throw new DomainException("Invalid operation task ID");

        ShiftLogId = operationTaskId;
        StartTime = startTime;
        EventType = eventType;
    }

    /// <summary>
    /// Kết thúc sự kiện (ví dụ: kết thúc nghỉ ca, kết thúc sự cố)
    /// </summary>
    public void EndEvent(DateTime endTime)
    {
        if (EndTime.HasValue)
            throw new DomainException("Event already ended");

        if (endTime < StartTime)
            throw new DomainException("End time cannot be before start time");

        EndTime = endTime;
        Duration = endTime - StartTime;
    }

    private ShiftLogEvent()
    {
        // EF Core constructor
    }
}

/// <summary>
/// Loại sự kiện của Event
/// </summary>
public enum ShiftLogEventType
{
    Break = 0,
    Breakdown = 1,
}
