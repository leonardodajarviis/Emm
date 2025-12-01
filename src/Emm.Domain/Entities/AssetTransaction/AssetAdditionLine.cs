namespace Emm.Domain.Entities.AssetTransaction;


public class AssetAdditionLine
{
    public long Id { get; private set; }
    public long AssetAdditionId { get; private set; }
    public long AssetModelId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }

    public AssetAdditionLine(
        long assetAdditionId,
        long assetModelId,
        string assetCode,
        decimal unitPrice)
    {
        AssetAdditionId = assetAdditionId;
        AssetModelId = assetModelId;
        UnitPrice = unitPrice;
        AssetCode = assetCode;
    }

    public AssetAdditionLine(){}
}   