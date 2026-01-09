using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities;

public class ParameterCatalog : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid UnitOfMeasureId { get; private set; }
    public ParameterType Type { get; private set; }
    public string? Description { get; private set; }
    public bool IsCodeGenerated { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;
    public ParameterCatalog(string code, string name, Guid unitOfMeasureId, string? description = null)
    {
        Code = code;
        Name = name;
        Description = description;
        UnitOfMeasureId = unitOfMeasureId;
    }

    public void Update(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    private ParameterCatalog() { } // EF Core constructor
}

public enum ParameterType
{
    Snapshot,
    Cumulative
}
