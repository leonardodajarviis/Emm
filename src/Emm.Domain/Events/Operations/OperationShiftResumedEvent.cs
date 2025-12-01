using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is resumed from paused state
/// </summary>
public sealed record OperationShiftResumedEvent : IDomainEvent
{
    public long ShiftId { get; init; }
    public string? Notes { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftResumedEvent(long shiftId, string? notes)
    {
        ShiftId = shiftId;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
    }
}
