using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Organization;

public class Location : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public long OrganizationUnitId { get; private set; }
    public bool IsActive { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    public Location(string code, string name, long organizationUnitId, string? description = null, bool isActive = true)
    {
        Code = code;
        Name = name;
        OrganizationUnitId = organizationUnitId;
        Description = description;
        IsActive = isActive;
    }

    public void Update(string name, string? description, bool isActive)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    private Location() { } // EF Core constructor
}
