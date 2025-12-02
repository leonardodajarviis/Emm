using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Dtos;

public record OperationShiftSummaryResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long OrganizationUnitId { get; set; }
    public required long PrimaryUserId { get; set; }
    public required bool IsCheckpointLogEnabled {get; set;}
    public string? PrimaryUserDisplayName { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public required OperationShiftStatus Status { get; set; }
    public string? Notes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
public record OperationShiftResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long OrganizationUnitId { get; set; }
    public required long PrimaryUserId { get; set; }
    public required bool IsCheckpointLogEnabled {get; set;}
    public string? PrimaryUserDisplayName { get; set; }
    public required DateTime ScheduledStartTime { get; set; }
    public required DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public required OperationShiftStatus Status { get; set; }
    public string? Notes { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }

    // Related data
    public List<OperationShiftAssetResponse> Assets { get; set; } = [];
    public List<OperationTaskResponse> ShiftLogs { get; set; } = [];
}

public record OperationShiftAssetParameterResponse
{
    public required long AssetId { get; set; }
    public required long ParameterId { get; set; }
    public string? ParameterCode { get; set; }
    public string? ParameterName { get; set; }
    public string? ParameterUnit { get; set; }
}

public record OperationShiftAssetResponse
{
    public required long Id { get; set; }
    public required long OperationShiftId { get; set; }
    public required long AssetId { get; set; }
    public required string AssetCode { get; set; }
    public required string AssetName { get; set; }
    public required bool IsPrimary { get; set; }
    public IEnumerable<OperationShiftAssetParameterResponse> Parameters { get; set; } = [];
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}

public record OperationTaskResponse
{
    public required long Id { get; set; }
    public required long OperationShiftId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public IReadOnlyCollection<OperationShfitTaskCheckpointResponse> Checkpoints { get; set; } = [];
    public IReadOnlyCollection<OperationShiftParameterReadingResponse> Readings { get; set; } = [];
    public IReadOnlyCollection<OperationShiftItemResponse> Items { get; set; } = [];
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
}

public record OperationShiftParameterReadingResponse
{
    public long Id { get; set; }
    public long OperationTaskId { get; set; }
    public long AssetId { get; set; }
    public long? TaskCheckpointId { get; set; }
    public Guid? ShiftLogCheckpointLinkedId { get; set; }
    public string AssetCode { get; set; } = null!;
    public string AssetName { get; set; } = null!;
    public long ParameterId { get; set; }
    public string ParameterName { get; set; } = null!;
    public string ParameterCode { get; set; } = null!;
    public decimal Value { get; set; }
    public string? StringValue { get; set; }
    public string Unit { get; set; } = null!;
    public DateTime ReadingTime { get; set; }
    public string? Notes { get; set; }
}

public class OperationShfitTaskCheckpointResponse
{
    public long Id { get; set; }
    public long OperationTaskId { get; set; }
    public Guid LinkedId { get; set; }
    public long LocationId { get; set; }
    public string? LocationName { get; set; }
    public bool IsWithAttachedMaterial { get; set; }
    public long? ItemId { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
}

public class OperationShiftItemResponse
{
    public long Id { get; set; }
    public long ShiftLogId { get; set; }
    public long ItemId { get; set; }
    public string ItemName { get; set; } = null!;
    public decimal Quantity { get; set; }
    public long? AssetId { get; set; }
    public string? AssetCode { get; set; }
    public string? AssetName { get; set; }
    public long? UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
}
