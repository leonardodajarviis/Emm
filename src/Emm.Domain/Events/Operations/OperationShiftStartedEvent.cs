using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is started
/// </summary>
public sealed record OperationShiftStartedEvent : IDomainEvent
{
    public long ShiftId { get; init; }
    public DateTime ActualStartTime { get; init; }
    public long PrimaryEmployeeId { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftStartedEvent(
        long shiftId,
        DateTime actualStartTime,
        long primaryEmployeeId)
    {
        ShiftId = shiftId;
        ActualStartTime = actualStartTime;
        PrimaryEmployeeId = primaryEmployeeId;
        OccurredOn = DateTime.UtcNow;
    }
}
