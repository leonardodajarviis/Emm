using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Dtos;

public record OperationShiftSummaryResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Guid OrganizationUnitId { get; set; }
    public required Guid PrimaryUserId { get; set; }
    public required bool IsCheckpointLogEnabled { get; set; }
    public string? PrimaryUserDisplayName { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public required string Status { get; set; }
    public string? Notes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ModifiedAt { get; set; }
}

public record OperationShiftResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Guid OrganizationUnitId { get; set; }
    public required Guid PrimaryUserId { get; set; }
    public required bool IsCheckpointLogEnabled { get; set; }
    public string? PrimaryUserDisplayName { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public required string Status { get; set; }
    public string? Notes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ModifiedAt { get; set; }

    // Related data
    public List<OperationShiftAssetResponse> Assets { get; set; } = [];
    public List<OperationShiftAssetBoxResponse> AssetBoxes { get; set; } = [];
    public List<ShiftLogResponse> ShiftLogs { get; set; } = [];
}

public record OperationShiftAssetParameterResponse
{
    public required Guid AssetId { get; init; }
    public required Guid ParameterId { get; init; }
    public required string ParameterCode { get; init; }
    public required string ParameterName { get; init; }
    public required Guid UnitOfMeasureId { get; set; }
    public required string UnitOfMeasureName { get; set; }
    public required string UnitOfMeasureSymbol { get; set; }
}

public record OperationShiftAssetResponse
{
    public required Guid Id { get; set; }
    public required Guid OperationShiftId { get; set; }
    public required Guid AssetId { get; set; }
    public required string AssetCode { get; set; }
    public required string AssetName { get; set; }
    public required bool IsPrimary { get; set; }
    public Guid? AssetBoxId { get; set; }
    public IEnumerable<OperationShiftAssetParameterResponse> Parameters { get; set; } = [];
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}

public record OperationShiftAssetBoxResponse
{
    public required Guid Id { get; set; }
    public required Guid OperationShiftId { get; set; }
    public required string BoxName { get; set; }
    public string? Description { get; set; }
    public required BoxRole Role { get; set; }
    public required int DisplayOrder { get; set; }
}

public record ShiftLogResponse
{
    public required Guid Id { get; set; }
    public required Guid OperationShiftId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid? AssetId { get; set; }
    public Guid? BoxId { get; set; }
    public IReadOnlyCollection<ShiftLogEventResponse> Events { get; set; } = [];
    public IReadOnlyCollection<ShiftLogCheckpointResponse> Checkpoints { get; set; } = [];
    public IReadOnlyCollection<ShiftLogParameterReadingResponse> Readings { get; set; } = [];
    public IReadOnlyCollection<ShiftLogItemResponse> Items { get; set; } = [];
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
}

public record ShiftLogParameterReadingResponse
{
    public Guid Id { get; init; }
    public Guid OperationTaskId { get; init; }
    public Guid AssetId { get; init; }
    public Guid? TaskCheckpointId { get; init; }
    public Guid? ShiftLogCheckpointLinkedId { get; init; }
    public string AssetCode { get; init; } = null!;
    public string AssetName { get; init; } = null!;
    public Guid ParameterId { get; init; }
    public string ParameterName { get; init; } = null!;
    public string ParameterCode { get; init; } = null!;
    public decimal Value { get; init; }
    public string? StringValue { get; init; }
    public Guid UnitOfMeasureId { get; init; }
    public string UnitOfMeasureName { get; init; } = null!;
    public string UnitOfMeasureSymbol { get; init; } = null!;
    public DateTime ReadingTime { get; init; }
    public string? Notes { get; init; }
}

public class ShiftLogCheckpointResponse
{
    public Guid Id { get; set; }
    public Guid OperationTaskId { get; set; }
    public Guid LinkedId { get; set; }
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public bool IsWithAttachedMaterial { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
}

public class ShiftLogItemResponse
{
    public Guid Id { get; set; }
    public Guid ShiftLogId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public Guid? AssetId { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public Guid? UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
}

public class ShiftLogEventResponse
{
    public Guid Id { get; set; }
    public Guid ShiftLogId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int EventType { get; set; }
}
