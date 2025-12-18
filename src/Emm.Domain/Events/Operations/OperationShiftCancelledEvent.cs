using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is cancelled
/// </summary>
public sealed record OperationShiftCancelledEvent : IDomainEvent
{
    public Guid ShiftId { get; init; }
    public string Reason { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftCancelledEvent(
        Guid shiftId,
        string reason)
    {
        ShiftId = shiftId;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}
