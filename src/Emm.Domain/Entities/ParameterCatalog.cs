using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities;

public class ParameterCatalog: AggregateRoot
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public long UnitOfMeasureId { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ParameterCatalog(string code, string name, string? description = null)
    {
        Code = code;
        Name = name;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private ParameterCatalog() { } // EF Core constructor
}
