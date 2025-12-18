using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is rescheduled
/// </summary>
public sealed record OperationShiftRescheduledEvent : IDomainEvent
{
    public Guid ShiftId { get; init; }
    public DateTime OldScheduledStartTime { get; init; }
    public DateTime OldScheduledEndTime { get; init; }
    public DateTime NewScheduledStartTime { get; init; }
    public DateTime NewScheduledEndTime { get; init; }
    public string Reason { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftRescheduledEvent(
        Guid shiftId,
        DateTime oldScheduledStartTime,
        DateTime oldScheduledEndTime,
        DateTime newScheduledStartTime,
        DateTime newScheduledEndTime,
        string reason)
    {
        ShiftId = shiftId;
        OldScheduledStartTime = oldScheduledStartTime;
        OldScheduledEndTime = oldScheduledEndTime;
        NewScheduledStartTime = newScheduledStartTime;
        NewScheduledEndTime = newScheduledEndTime;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}
