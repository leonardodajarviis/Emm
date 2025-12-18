using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Organization;

public class OrganizationUnit : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? ParentId { get; private set; }
    public Guid OrganizationUnitLevelId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public OrganizationUnit(
        string code,
        string name,
        Guid organizationUnitLevelId,
        string? description = null,
        bool isActive = true,
        Guid? parentId = null)
    {
        Code = code;
        Name = name;
        OrganizationUnitLevelId = organizationUnitLevelId;
        Description = description;
        IsActive = isActive;
        ParentId = parentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(
        string name,
        Guid organizationUnitLevelId,
        string? description = null,
        bool isActive = true,
        Guid? parentId = null)
    {
        Name = name;
        OrganizationUnitLevelId = organizationUnitLevelId;
        Description = description;
        IsActive = isActive;
        ParentId = parentId;
        UpdatedAt = DateTime.UtcNow;
    }

    private OrganizationUnit() { } // EF Core constructor
}