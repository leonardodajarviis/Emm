using Emm.Domain.Abstractions;
using Emm.Domain.Events.AssetAddition;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetTransaction;

public class AssetAddition : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public Guid OrganizationUnitId { get; private set; }
    public Guid LocationId { get; private set; }
    public string? DecisionNumber { get; private set; }
    public DateTime? DecisionDate { get; private set; }
    public string? Reason { get; private set; }
    private readonly List<AssetAdditionLine> _assetAdditionLines = [];
    public IReadOnlyList<AssetAdditionLine> AssetAdditionLines => _assetAdditionLines.AsReadOnly();

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    public AssetAddition(
        string code,
        Guid organizationUnitId,
        Guid locationId,
        string? decisionNumber,
        DateTime? decisionDate,
        string? reason)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty", nameof(code));

        if (organizationUnitId == Guid.Empty)
            throw new ArgumentException("OrganizationUnitId cannot be empty", nameof(organizationUnitId));

        if (locationId == Guid.Empty)
            throw new ArgumentException("LocationId cannot be empty", nameof(locationId));

        Code = code;
        OrganizationUnitId = organizationUnitId;
        LocationId = locationId;
        DecisionNumber = decisionNumber;
        DecisionDate = decisionDate;
        Reason = reason;
    }

    public void AddAssetAdditionLine(string assetCode, Guid assetModelId, decimal unitPrice)
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
