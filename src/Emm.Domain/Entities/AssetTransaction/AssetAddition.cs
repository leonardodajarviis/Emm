using Emm.Domain.Abstractions;
using Emm.Domain.DomainEvents.AssetAdditionEvents;

namespace Emm.Domain.Entities.AssetTransaction;

public class AssetAddition : AggregateRoot
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public long OrganizationUnitId { get; private set; }
    public long LocationId { get; private set; }
    public string? DecisionNumber { get; private set; }
    public string? DecisionDate { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<AssetAdditionLine> _assetAdditionLines = [];
    public IReadOnlyList<AssetAdditionLine> AssetAdditionLines => _assetAdditionLines.AsReadOnly();

    public AssetAddition(
        string code,
        long organizationUnitId,
        long locationId,
        string? decisionNumber,
        string? decisionDate,
        string? reason,
        DateTime createdAt)
    {
        Code = code;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;
        DecisionNumber = decisionNumber;
        DecisionDate = decisionDate;
        Reason = reason;
        CreatedAt = createdAt;
    }

    public void AddAssetAdditionLine(string assetCode, long assetModelId, decimal unitPrice)
    {
        var line = new AssetAdditionLine(Id, assetModelId, assetCode, unitPrice);
        _assetAdditionLines.Add(line);
    }

    public void RegisterEvent()
    {
        Raise(new AssetAdditionCreatedEvent(Id, LocationId, OrganizationUnitId, [.. _assetAdditionLines.Select(line => new AssetAdditionCreatedEventAssetLine(line.AssetModelId, line.AssetCode))]));
    }
}