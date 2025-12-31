using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetTransaction;


public class AssetAdditionLine
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid AssetAdditionId { get; private set; }
    public Guid AssetModelId { get; private set; }
    public string AssetDisplayName { get; private set; } = null!;
    public NaturalKey AssetCode { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public decimal UnitPrice { get; private set; }

    // Navigation property
    public AssetAddition AssetAddition { get; private set; } = null!;

    public AssetAdditionLine(
        Guid assetModelId,
        bool isCodeGenerated,
        NaturalKey assetCode,
        string assetDisplayName,
        decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentException("Unit price must be non-negative", nameof(unitPrice));

        AssetModelId = assetModelId;
        AssetCode = assetCode;
        UnitPrice = unitPrice;
        AssetDisplayName = assetDisplayName;
        IsCodeGenerated = isCodeGenerated;
    }

    // EF Core constructor
    private AssetAdditionLine() { }

    internal void SetAssetAddition(AssetAddition assetAddition)
    {
        AssetAddition = assetAddition ?? throw new ArgumentNullException(nameof(assetAddition));
    }
}
