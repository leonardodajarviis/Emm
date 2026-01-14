using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

public class Item : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid GroupId { get; private set; }
    public Guid UnitOfMeasureId { get; private set; }
    public Guid? UnitOfMeasureCategoryId { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;


    public Item(string code, string name, Guid groupId, Guid unitOfMeasureId, Guid unitOfMeasureCategoryId)
    {
        Code = code;
        Name = name;
        UnitOfMeasureId = unitOfMeasureId;
        GroupId = groupId;
        UnitOfMeasureCategoryId = unitOfMeasureCategoryId;
    }

    private Item() { } // EF Core constructor
}
