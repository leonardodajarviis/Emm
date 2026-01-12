namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Điểm kiểm tra trong nhiệm vụ vận hành
/// </summary>
public class ShiftLogCheckpoint
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid LinkedId { get; private set; }
    public Guid ShiftLogId { get; private set; }
    public int CheckpointOrder { get; private set; }
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
        string locationName
    )
    {
        LinkedId = linkedId;
        LocationId = locationId;
        LocationName = locationName;

        ShiftLogId = operationTaskId;
        Name = name;
    }

    public void MakeOrder(int order)
    {
        CheckpointOrder = order;
    }

    public void MakeAttachedMaterial(Guid itemId, string itemCode, string itemName)
    {
        IsWithAttachedMaterial = true;
        ItemId = itemId;
        ItemCode = itemCode;
        ItemName = itemName;
    }

    public void UpdateLocation(Guid locationId,  string locationName)
    {
        LocationId = locationId;
        LocationName = locationName;
    }

    private ShiftLogCheckpoint()
    {
    } // EF Core constructor
}
