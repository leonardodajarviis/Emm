using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Tài sản tham gia trong ca vận hành
/// </summary>
public class OperationShiftAsset
{
    public long Id { get; private set; }
    public long OperationShiftId { get; private set; }
    public long AssetId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public string AssetName { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public AssetStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    public OperationShiftAsset(
        long operationShiftId,
        long assetId,
        string assetCode,
        string assetName,
        bool isPrimary = false)
    {
        OperationShiftId = operationShiftId;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        IsPrimary = isPrimary;
        Status = AssetStatus.Scheduled;
    }

    public void StartOperation(string? notes = null)
    {
        if (Status != AssetStatus.Scheduled && Status != AssetStatus.Standby)
            throw new DomainException($"Cannot start asset operation in {Status} status. Asset must be in Scheduled or Standby status.");

        Status = AssetStatus.InOperation;
        StartedAt = DateTime.UtcNow;
        Notes = notes;
    }

    public void CompleteOperation(string? notes = null)
    {
        if (Status != AssetStatus.InOperation)
            throw new DomainException($"Cannot complete asset operation in {Status} status. Asset must be in InOperation status.");

        Status = AssetStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(notes))
            Notes = notes;
    }

    public void SetMaintenance(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Maintenance reason is required");

        if (Status == AssetStatus.Completed)
            throw new DomainException("Cannot set maintenance status for completed asset");

        Status = AssetStatus.Maintenance;
        Notes = reason;
    }

    public void SetFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Failure reason is required");

        if (Status == AssetStatus.Completed)
            throw new DomainException("Cannot set failed status for completed asset");

        Status = AssetStatus.Failed;
        Notes = reason;
    }

    public void SetStandby(string? reason = null)
    {
        if (Status == AssetStatus.InOperation)
            throw new DomainException("Cannot set standby status while asset is in operation. Complete or fail the operation first.");

        if (Status == AssetStatus.Completed)
            throw new DomainException("Cannot set standby status for completed asset");

        Status = AssetStatus.Standby;
        Notes = reason;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    private OperationShiftAsset() { } // EF Core constructor
}

public record OperationShiftAssetSpec
{
    public long OperationShiftId { get; init; }
    public long AssetId { get; init; }
    public string AssetCode { get; init; } = null!;
    public string AssetName { get; init; } = null!;
    public string AssetType { get; init; } = null!;
    public bool IsPrimary { get; init; }
}

public enum AssetStatus
{
    Scheduled = 0,
    InOperation = 1,
    Completed = 2,
    Maintenance = 3,
    Failed = 4,
    Standby = 5
}
