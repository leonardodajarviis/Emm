namespace Emm.Domain.Entities.Operations;

public class ShiftLogItem
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid ShiftLogId { get; private set; }
    public Guid? WarehouseIssueSlipId { get; private set; }

    public Guid? AssetId { get; private set; }
    public string? AssetCode { get; private set; }
    public string? AssetName { get; private set; }

    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public string ItemCode { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public Guid? UnitOfMeasureId { get; private set; }
    public string? UnitOfMeasureName { get; private set; }

    public ShiftLogItem(
        Guid shiftLogId,
        Guid? warehouseIssueSlipId,
        Guid itemId,
        string itemCode,
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
        ItemCode = itemCode;
        Quantity = quantity;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        UnitOfMeasureId = unitOfMeasureId;
        UnitOfMeasureName = unitOfMeasureName;
        WarehouseIssueSlipId = warehouseIssueSlipId;
    }

    public void UpdateQuantity(decimal quantity)
    {
        Quantity = quantity;
    }

    private ShiftLogItem() { } // EF Core constructor
}
