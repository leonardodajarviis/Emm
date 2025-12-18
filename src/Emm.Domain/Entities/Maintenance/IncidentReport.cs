using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Maintenance;

/// <summary>
/// Aggregate Root representing an Incident Report (Phiếu báo sự cố).
/// </summary>
public class IncidentReport : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid AssetId { get; private set; }
    public DateTime ReportedAt { get; private set; }
    public IncidentPriority Priority { get; private set; }
    public IncidentStatus Status { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    public IncidentReport(
        string code,
        string title,
        string description,
        Guid assetId,
        IncidentPriority priority
    )
    {
        if (string.IsNullOrWhiteSpace(code)) throw new DomainException("Code is required");
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description is required");
        DomainGuard.AgainstInvalidForeignKey(assetId, nameof(AssetId));

        Code = code;
        Title = title;
        Description = description;
        AssetId = assetId;
        Priority = priority;
        Status = IncidentStatus.New;
        ReportedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string title, string description, IncidentPriority priority)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description is required");

        Title = title;
        Description = description;
        Priority = priority;
    }

    public void Assign()
    {
        if (Status == IncidentStatus.Resolved || Status == IncidentStatus.Closed)
            throw new DomainException("Cannot assign a resolved or closed incident.");

        Status = IncidentStatus.Assigned;
    }

    public void StartProgress()
    {
        if (Status == IncidentStatus.Resolved || Status == IncidentStatus.Closed)
            throw new DomainException("Cannot start progress on a resolved or closed incident.");

        Status = IncidentStatus.InProgress;
    }

    public void Resolve(string resolutionNotes)
    {
        if (string.IsNullOrWhiteSpace(resolutionNotes)) throw new DomainException("Resolution notes are required");

        ResolutionNotes = resolutionNotes;
        ResolvedAt = DateTime.UtcNow;
        Status = IncidentStatus.Resolved;
    }

    public void Close()
    {
        Status = IncidentStatus.Closed;
    }

    private IncidentReport() { }
}

public enum IncidentPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}

public enum IncidentStatus
{
    New = 0,
    Assigned = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4
}
