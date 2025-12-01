using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when a task is completed
/// </summary>
public sealed record OperationShiftTaskCompletedEvent : IDomainEvent
{
    public long TaskId { get; init; }
    public long OperationShiftId { get; init; }
    public DateTime CompletedAt { get; init; }
    public string? Notes { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftTaskCompletedEvent(
        long taskId,
        long operationShiftId,
        DateTime completedAt,
        string? notes = null)
    {
        TaskId = taskId;
        OperationShiftId = operationShiftId;
        CompletedAt = completedAt;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
    }
}
