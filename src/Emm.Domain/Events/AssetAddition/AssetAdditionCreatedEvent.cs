using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Events.AssetAddition;


public record AssetAdditionCreatedEvent: IImmediateDomainEvent
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
    public NaturalKey AssetCode { get; }
    public string AssetDisplayName { get; }
    public bool IsCodeGenerated { get; }

    public AssetAdditionCreatedEventAssetLine(Guid assetModelId, bool isCodeGenerated, NaturalKey assetCode, string assetDisplayName)
    {
        AssetModelId = assetModelId;
        AssetDisplayName = assetDisplayName;
        IsCodeGenerated = isCodeGenerated;
        AssetCode = assetCode;
    }
}
