using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities;

public class ParameterCatalog: AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public long UnitOfMeasureId { get; private set; }
    public string? Description { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public long? UpdatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ParameterCatalog(string code, string name, long unitOfMeasureId, string? description = null)
    {
        Code = code;
        Name = name;
        Description = description;
        UnitOfMeasureId = unitOfMeasureId;
    }

    public void SetAuditInfo(long? createdByUserId, long? updatedByUserId)
    {
        CreatedByUserId = createdByUserId;
        UpdatedByUserId = updatedByUserId;
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private ParameterCatalog() { } // EF Core constructor
}
