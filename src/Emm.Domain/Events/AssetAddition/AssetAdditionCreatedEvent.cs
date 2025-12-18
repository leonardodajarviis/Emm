using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.AssetAddition;

public record AssetAdditionCreatedEvent : IDomainEvent
{
    public Guid AssetAdditionId { get; }
    public Guid LocationId { get; }
    public Guid OrganizationUnitId { get; }
    public IReadOnlyCollection<AssetAdditionCreatedEventAssetLine> AssetLines { get; } = [];
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public AssetAdditionCreatedEvent(Guid assetAdditionId, Guid locationId, Guid organizationUnitId, IReadOnlyCollection<AssetAdditionCreatedEventAssetLine> assetLines)
    {
        AssetAdditionId = assetAdditionId;
        LocationId = locationId;
        OrganizationUnitId = organizationUnitId;
        AssetLines = assetLines;
    }
}

public sealed record AssetAdditionCreatedEventAssetLine
{
    public Guid AssetModelId { get; }
    public string AssetCode { get; }

    public AssetAdditionCreatedEventAssetLine(Guid assetModelId, string assetCode)
    {
        AssetModelId = assetModelId;
        AssetCode = assetCode;
    }
}
