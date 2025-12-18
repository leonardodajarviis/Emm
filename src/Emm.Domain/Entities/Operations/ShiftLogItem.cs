namespace Emm.Domain.Entities.Operations;

public class ShiftLogItem
{
    public Guid Id { get; private set; }
    public Guid ShiftLogId { get; private set; }

    public Guid? AssetId { get; private set; }
    public string? AssetCode { get; private set; }
    public string? AssetName { get; private set; }

    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public Guid? UnitOfMeasureId { get; private set; }
    public string? UnitOfMeasureName { get; private set; }

    public ShiftLogItem(
        Guid shiftLogId,
        Guid itemId,
        string itemName,
        decimal quantity,
        Guid? assetId = null,
        string? assetCode = null,
        string? assetName = null,
        Guid? unitOfMeasureId = null,
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
