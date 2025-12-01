using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is completed
/// </summary>
public sealed record OperationShiftCompletedEvent : IDomainEvent
{
    public long ShiftId { get; init; }
    public DateTime ActualEndTime { get; init; }
    public long PrimaryEmployeeId { get; init; }
    public string? Notes { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftCompletedEvent(
        long shiftId,
        DateTime actualEndTime,
        long primaryEmployeeId,
        string? notes = null)
    {
        ShiftId = shiftId;
        ActualEndTime = actualEndTime;
        PrimaryEmployeeId = primaryEmployeeId;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
    }
}
