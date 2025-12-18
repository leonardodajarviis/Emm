using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when a cancelled operation shift is reactivated
/// </summary>
public sealed record OperationShiftReactivatedEvent : IDomainEvent
{
    public Guid ShiftId { get; init; }
    public DateTime NewScheduledStartTime { get; init; }
    public DateTime NewScheduledEndTime { get; init; }
    public string Reason { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftReactivatedEvent(
        Guid shiftId,
        DateTime newScheduledStartTime,
        DateTime newScheduledEndTime,
        string reason)
    {
        ShiftId = shiftId;
        NewScheduledStartTime = newScheduledStartTime;
        NewScheduledEndTime = newScheduledEndTime;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}
