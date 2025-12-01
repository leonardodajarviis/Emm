namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Điểm kiểm tra trong nhiệm vụ vận hành
/// </summary>
public class ShiftLogCheckpoint
{
    public long Id { get; private set; }
    public Guid LinkedId { get; private set; }
    public long ShiftLogId { get; private set; }
    public string Name { get; private set; } = null!;
    public long LocationId { get; private set; }
    public string LocationName { get; private set; } = null!;

    public long? ItemId { get; private set; }
    public string? ItemCode { get; private set; }
    public string? ItemName { get; private set; }
    public bool IsWithAttachedMaterial { get; private set; }

    public ShiftLogCheckpoint(
        long operationTaskId,
        Guid linkedId,
        string name,
        long locationId,
        string locationName,
        bool isWithAttachedMaterial = false,
        long? itemId = null,
        string? itemCode = null,
        string? itemName = null)
    {
        LinkedId = linkedId;
        LocationId = locationId;
        LocationName = locationName;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
        IsWithAttachedMaterial = isWithAttachedMaterial;

        ShiftLogId = operationTaskId;
        Name = name;
    }

    /// <summary>
    /// Update checkpoint properties
    /// </summary>
    public void Update(string name, long locationId, string locationName, bool isWithAttachedMaterial = false, long? itemId = null, string? itemCode = null, string? itemName = null)
    {
        Name = name;
        LocationId = locationId;
        LocationName = locationName;
        IsWithAttachedMaterial = isWithAttachedMaterial;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
    }

    private ShiftLogCheckpoint()
    {
    } // EF Core constructor
}

public enum CheckpointStatus
{
    Pending = 0,
    Completed = 1,
    Skipped = 2,
    Failed = 3
}
