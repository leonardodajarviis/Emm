namespace Emm.Domain.Entities.Operations;

public class ShiftLogItem
{
    public long Id { get; private set; }
    public long ShiftLogId { get; private set; }

    public long? AssetId { get; private set; }
    public string? AssetCode { get; private set; }
    public string? AssetName { get; private set; }

    public long ItemId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public long? UnitOfMeasureId { get; private set; }
    public string? UnitOfMeasureName { get; private set; }

    public ShiftLogItem(
        long shiftLogId,
        long itemId,
        string itemName,
        decimal quantity,
        long? assetId = null,
        string? assetCode = null,
        string? assetName = null,
        long? unitOfMeasureId = null,
        string? unitOfMeasureName = null)
    {
        ShiftLogId = shiftLogId;
        ItemId = itemId;
        ItemName = itemName;
        Quantity = quantity;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        UnitOfMeasureId = unitOfMeasureId;
        UnitOfMeasureName = unitOfMeasureName;
    }

    private ShiftLogItem() { } // EF Core constructor
}
