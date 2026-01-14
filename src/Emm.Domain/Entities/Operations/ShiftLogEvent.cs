using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Lịch sử trạng thái và sự kiện của Task (nghỉ ca, sự cố, ghi chú)
/// </summary>
public class ShiftLogEvent
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid ShiftLogId { get; private set; }
    public int EventOrder { get; private set; }
    public ShiftLogEventType EventType { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public TimeSpan? Duration { get; private set; }
    public Guid? IncidentId { get; private set; }

    public ShiftLogEvent(
        Guid operationTaskId,
        ShiftLogEventType eventType,
        DateTime startTime,
        DateTime endTime)
    {
        DomainGuard.AgainstBusinessRule(
            endTime < startTime,
            "ShiftLogEvent.EndTimeBeforeStartTime",
            "Thời gian kết thúc không thể trước thời gian bắt đầu.");

        ShiftLogId = operationTaskId;
        StartTime = startTime;
        EventType = eventType;
        EndTime = endTime;
        Duration = EndTime - StartTime;
    }

    public void Update(
        ShiftLogEventType eventType,
        DateTime startTime,
        DateTime endTime)
    {
        DomainGuard.AgainstBusinessRule(
            endTime < startTime,
            "ShiftLogEvent.EndTimeBeforeStartTime",
            "Thời gian kết thúc không thể trước thời gian bắt đầu.");

        EventType = eventType;
        StartTime = startTime;
        EndTime = endTime;
        Duration = EndTime - StartTime;
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
