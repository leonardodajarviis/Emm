namespace Emm.Domain.Entities.AssetTransaction;


public class AssetAdditionLine
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid AssetAdditionId { get; private set; }
    public Guid AssetModelId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }

    // Navigation property
    public AssetAddition AssetAddition { get; private set; } = null!;

    public AssetAdditionLine(
        Guid assetModelId,
        string assetCode,
        decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(assetCode))
            throw new ArgumentException("Asset code cannot be empty", nameof(assetCode));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price must be non-negative", nameof(unitPrice));

        AssetModelId = assetModelId;
        AssetCode = assetCode;
        UnitPrice = unitPrice;
    }

    // EF Core constructor
    private AssetAdditionLine() { }

    internal void SetAssetAddition(AssetAddition assetAddition)
    {
        AssetAddition = assetAddition ?? throw new ArgumentNullException(nameof(assetAddition));
    }
}
