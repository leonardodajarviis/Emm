namespace Emm.Domain.Entities.Operations.CreationData;

public sealed record ShiftLogItemData
{
    public Guid ItemId { get; init; }
    public string ItemName { get; init; } = null!;
    public decimal Quantity { get; init; }
    public Guid? AssetId { get; init; }

}
