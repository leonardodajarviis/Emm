namespace Emm.Domain.Entities.Operations.CreationData;
public sealed record AssignAssetData
{
    public Guid AssetId { get; init; }
    public string AssetCode { get; init; } = null!;
    public string AssetName { get; init; } = null!;
    public bool IsPrimary { get; init; }
}
