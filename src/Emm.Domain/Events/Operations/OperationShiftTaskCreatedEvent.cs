using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift task is created
/// </summary>
public sealed record OperationShiftTaskCreatedEvent : IDomainEvent
{
    public long OperationShiftId { get; init; }
    public string TaskName { get; init; }
    public int Order { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftTaskCreatedEvent(
        long operationShiftId,
        string taskName)
    {
        OperationShiftId = operationShiftId;
        TaskName = taskName;
        OccurredOn = DateTime.UtcNow;
    }
}
