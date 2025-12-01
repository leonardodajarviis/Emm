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
}
