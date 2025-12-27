using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Inventory;

public class ItemGroup : AggregateRoot, IAuditableEntity
{
    public NaturalKey Code { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;


    public ItemGroup(
        NaturalKey code,
        string name,
        string? description = null)
    {
        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(
        string name,
        string? description = null)
    {
        Name = name;
        Description = description;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

}
