using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateShiftLogCommand : IRequest<Result>
{
    public Guid OperationShiftId { get; set; }

    public required CreateShiftLogData Data { get; set; }
}

public record CreateShiftLogData
{
    public string Name { get; init; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Asset ID cho trường hợp ghi log cho 1 asset cụ thể
    /// </summary>
    public Guid? AssetId { get; set; }

    /// <summary>
    /// Box ID cho trường hợp ghi log cho nhiều assets theo group
    /// </summary>
    public Guid? BoxId { get; set; }

    public Guid? LocationId {get; set;}
    public string? LocationName {get; set;}

    public IEnumerable<ParameterReadingRequest> Readings { get; set; } = [];
    public IEnumerable<CheckpointRequest> Checkpoints { get; set; } = [];
    public IEnumerable<LogEventRequest> Events { get; set; } = [];
    public IEnumerable<ShiftLogItemRequest> Items { get; set; } = [];
}

public sealed record ParameterReadingRequest
{
    public Guid? Id { get; init; }
    public Guid AssetId { get; init; }
    public Guid ParameterId { get; init; }
    public decimal Value { get; init; }
    public Guid? CheckpointLinkedId { get; init; }
}


/// <summary>
/// Specification for creating checkpoints
/// </summary>
public sealed record CheckpointRequest
{
    public Guid? Id { get; init; }
    public Guid LinkedId { get; init; }
    public string Name { get; init; } = null!;
    public Guid LocationId { get; init; }
    public bool IsWithAttachedMaterial { get; init; }
    public Guid? ItemId { get; init; }
}

/// <summary>
/// Specification for creating status history events
/// </summary>
public sealed record LogEventRequest
{
    public Guid? Id { get; init; }
    public ShiftLogEventType EventType { get; init; }
    public Guid? IncidentId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
}

/// <summary>
/// Specification for creating shift log items
/// </summary>
public sealed record ShiftLogItemRequest
{
    public Guid? Id { get; init; }
    public Guid ItemId { get; init; }
    public Guid? WarehouseIssueSlipId { get; init; }
    public decimal Quantity { get; init; }
    public Guid AssetId { get; init; }
}
