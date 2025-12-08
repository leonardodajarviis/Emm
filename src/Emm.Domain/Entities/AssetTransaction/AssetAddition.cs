using Emm.Domain.Abstractions;
using Emm.Domain.Events.AssetAddition;

namespace Emm.Domain.Entities.AssetTransaction;

public class AssetAddition : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public long OrganizationUnitId { get; private set; }
    public long LocationId { get; private set; }
    public string? DecisionNumber { get; private set; }
    public DateTime? DecisionDate { get; private set; }
    public string? Reason { get; private set; }
    private readonly List<AssetAdditionLine> _assetAdditionLines = [];
    public IReadOnlyList<AssetAdditionLine> AssetAdditionLines => _assetAdditionLines.AsReadOnly();

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public long? UpdatedByUserId { get; private set; }


    public AssetAddition(
        string code,
        long organizationUnitId,
        long locationId,
        string? decisionNumber,
        DateTime? decisionDate,
        string? reason)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        if (organizationUnitId <= 0)
            throw new ArgumentException("OrganizationUnitId must be greater than 0", nameof(organizationUnitId));

        if (locationId <= 0)
            throw new ArgumentException("LocationId must be greater than 0", nameof(locationId));

        Code = code;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;
        DecisionNumber = decisionNumber;
        DecisionDate = decisionDate;
        Reason = reason;
    }

    public void AddAssetAdditionLine(string assetCode, long assetModelId, decimal unitPrice)
    {
        var line = new AssetAdditionLine(assetModelId, assetCode, unitPrice);
        line.SetAssetAddition(this);
        _assetAdditionLines.Add(line);
    }

    public void RegisterEvent()
    {
        Raise(new AssetAdditionCreatedEvent(Id, LocationId, OrganizationUnitId, [.. _assetAdditionLines.Select(line => new AssetAdditionCreatedEventAssetLine(line.AssetModelId, line.AssetCode))]));
    }
}
