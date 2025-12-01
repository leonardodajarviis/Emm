using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Organization;

public class OrganizationUnit : AggregateRoot
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public long? ParentId { get; private set; }
    public long OrganizationUnitLevelId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public OrganizationUnit(
        string code,
        string name,
        long organizationUnitLevelId,
        string? description = null,
        bool isActive = true,
        long? parentId = null)
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
        long organizationUnitLevelId,
        string? description = null,
        bool isActive = true,
        long? parentId = null)
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