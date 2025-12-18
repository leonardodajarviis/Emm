using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Organization;

public class OrganizationUnitLevel : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public int Level { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public OrganizationUnitLevel(string name, string? description = null, int level = 0)
    {
        Name = name;
        Description = description;
        Level = level;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string? description, int level)
    {
        Name = name;
        Description = description;
        Level = level;
        UpdatedAt = DateTime.UtcNow;
    }

    private OrganizationUnitLevel() { } // EF Core constructor
}