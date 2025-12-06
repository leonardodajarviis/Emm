namespace Emm.Domain.Entities.AssetTransaction;


public class AssetAdditionLine
{
    public long Id { get; private set; }
    public long AssetAdditionId { get; private set; }
    public long AssetModelId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }

    // Navigation property
    public AssetAddition AssetAddition { get; private set; } = null!;

    public AssetAdditionLine(
        long assetModelId,
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