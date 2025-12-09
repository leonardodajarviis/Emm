using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

public class Item : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public long UnitOfMeasureId { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;


    public Item(string code, string name, long unitOfMeasureId)
    {
        Code = code;
        Name = name;
        UnitOfMeasureId = unitOfMeasureId;
    }
}
