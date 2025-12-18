using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is paused
/// </summary>
public sealed record OperationShiftPausedEvent : IDomainEvent
{
    public Guid ShiftId { get; init; }
    public string Reason { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftPausedEvent(Guid shiftId, string reason)
    {
        ShiftId = shiftId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}
