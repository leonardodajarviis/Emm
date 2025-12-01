using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when a task is started
/// </summary>
public sealed record OperationShiftTaskStartedEvent : IDomainEvent
{
    public long TaskId { get; init; }
    public long OperationShiftId { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftTaskStartedEvent(
        long taskId,
        long operationShiftId,
        DateTime startedAt)
    {
        TaskId = taskId;
        OperationShiftId = operationShiftId;
        StartedAt = startedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
