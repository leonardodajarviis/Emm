using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddShiftLogCommand : IRequest<Result<object>>
{
    public long OperationShiftId { get; set; }
    public string Name { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public IEnumerable<ParameterReadingRequest>? Readings { get; set; }
    public IEnumerable<CheckpointRequest>? Checkpoints { get; set; }
    public IEnumerable<LogEventRequest>? Events { get; set; }
}
public sealed record ParameterReadingRequest
{
    public long? Id { get; init; }
    public long AssetId { get; init; }
    public long ParameterId { get; init; }
    public decimal Value { get; init; }
    public Guid? TaskCheckpointLinkedId { get; init; }
}


/// <summary>
/// Specification for creating checkpoints
/// </summary>
public sealed record CheckpointRequest
{
    public long? Id { get; init; }
    public Guid LinkedId { get; init; }
    public string Name { get; init; } = null!;
    public long LocationId { get; init; }
    public bool IsWithAttachedMaterial { get; init; }
    public long? ItemId { get; init; }
}

/// <summary>
/// Specification for creating status history events
/// </summary>
public sealed record LogEventRequest
{
    public long? Id { get; init; }
    public ShiftLogEventType EventType { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}
