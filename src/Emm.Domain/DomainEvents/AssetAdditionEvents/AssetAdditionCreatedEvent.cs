using Emm.Domain.Abstractions;

namespace Emm.Domain.DomainEvents.AssetAdditionEvents;

public record AssetAdditionCreatedEvent : IDomainEvent
{
    public long AssetAdditionId { get; }
    public long LocationId { get; init; }
    public long OrganizationUnitId { get; private set; }

    public IReadOnlyCollection<AssetAdditionCreatedEventAssetLine> AssetLines { get; } = [];

    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public AssetAdditionCreatedEvent(long assetAdditionId, long locationId, long organizationUnitId, IReadOnlyCollection<AssetAdditionCreatedEventAssetLine> assetLines)
    {
        AssetAdditionId = assetAdditionId;
        LocationId = locationId;
        OrganizationUnitId = organizationUnitId;
        AssetLines = assetLines;
    }
}

public sealed record AssetAdditionCreatedEventAssetLine
{
    public long AssetModelId { get; }
    public string AssetCode { get; }

    public AssetAdditionCreatedEventAssetLine(long assetModelId, string assetCode)
    {
        AssetModelId = assetModelId;
        AssetCode = assetCode;
    }
}
