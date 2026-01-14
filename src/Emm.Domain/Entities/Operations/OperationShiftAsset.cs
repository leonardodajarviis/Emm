namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Tài sản tham gia trong ca vận hành
/// </summary>
public class OperationShiftAsset
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid OperationShiftId { get; private set; }
    public Guid AssetId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public string AssetName { get; private set; } = null!;
    public bool IsPrimary { get; private set; }
    public bool IsCheckpointLogEnabled {get; private set; }
    public Guid? AssetBoxId { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    public OperationShiftAsset(
        Guid operationShiftId,
        Guid assetId,
        string assetCode,
        string assetName,
        bool isPrimary = false,
        Guid? assetBoxId = null)
    {
        OperationShiftId = operationShiftId;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        IsPrimary = isPrimary;
        AssetBoxId = assetBoxId;
    }

    public void AssignToGroup(Guid assetBoxId)
    {
        AssetBoxId = assetBoxId;
    }


    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    private OperationShiftAsset() { } // EF Core constructor
}
