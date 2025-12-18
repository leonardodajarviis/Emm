namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Điểm kiểm tra trong nhiệm vụ vận hành
/// </summary>
public class ShiftLogCheckpoint
{
    public Guid Id { get; private set; }
    public Guid LinkedId { get; private set; }
    public Guid ShiftLogId { get; private set; }
    public string Name { get; private set; } = null!;
    public Guid LocationId { get; private set; }
    public string LocationName { get; private set; } = null!;

    public Guid? ItemId { get; private set; }
    public string? ItemCode { get; private set; }
    public string? ItemName { get; private set; }
    public bool IsWithAttachedMaterial { get; private set; }

    public ShiftLogCheckpoint(
        Guid operationTaskId,
        Guid linkedId,
        string name,
        Guid locationId,
        string locationName,
        bool isWithAttachedMaterial = false,
        Guid? itemId = null,
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
    public void Update(string name, Guid locationId, string locationName, bool isWithAttachedMaterial = false, Guid? itemId = null, string? itemCode = null, string? itemName = null)
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
