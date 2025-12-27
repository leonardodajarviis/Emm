using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities;

public class UnitOfMeasureCategory : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;

    public Guid BaseUnitId { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;


    public UnitOfMeasureCategory(string code, string name)
    {
        Code = code;
        Name = name;
    }
}

public class UnitOfMeasureCategoryLine
{
    public Guid UnitOfMeasureCategoryId { get; private set; }
    public Guid UnitOfMeasureId { get; private set; }

    public UnitOfMeasureCategoryLine(Guid unitOfMeasureCategoryId, Guid unitOfMeasureId)
    {
        UnitOfMeasureCategoryId = unitOfMeasureCategoryId;
        UnitOfMeasureId = unitOfMeasureId;
    }
}
