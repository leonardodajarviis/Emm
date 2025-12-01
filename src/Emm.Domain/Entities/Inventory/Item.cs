using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Inventory;

public class Item : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public long UnitOfMeasureId { get; private set; }

    public DateTime CreatedAt {get; private set; }
    public DateTime UpdatedAt {get; private set; }


    public Item(string code, string name, long unitOfMeasureId)
    {
        Code = code;
        Name = name;
        UnitOfMeasureId = unitOfMeasureId;
    }
}
