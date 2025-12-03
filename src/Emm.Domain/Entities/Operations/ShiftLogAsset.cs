using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Liên kết giữa ShiftLog và Asset - cho phép 1 ShiftLog ghi nhận cho 1 hoặc nhiều asset
/// </summary>
public class ShiftLogAsset
{
    public long Id { get; private set; }
    public long ShiftLogId { get; private set; }
    public long AssetId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public string AssetName { get; private set; } = null!;

    /// <summary>
    /// Đánh dấu asset chính trong nhóm (nếu có nhiều asset)
    /// </summary>
    public bool IsPrimary { get; private set; }


    public ShiftLogAsset(
        long shiftLogId,
        long assetId,
        string assetCode,
        string assetName,
        bool isPrimary = false)
    {
        if (shiftLogId <= 0)
            throw new DomainException("Invalid shift log ID");

        if (assetId <= 0)
            throw new DomainException("Invalid asset ID");

        if (string.IsNullOrWhiteSpace(assetCode))
            throw new DomainException("Asset code is required");

        if (string.IsNullOrWhiteSpace(assetName))
            throw new DomainException("Asset name is required");

        ShiftLogId = shiftLogId;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        IsPrimary = isPrimary;
    }

    public void MarkAsPrimary()
    {
        IsPrimary = true;
    }

    public void UnmarkAsPrimary()
    {
        IsPrimary = false;
    }

    private ShiftLogAsset() { } // EF Core constructor
}
