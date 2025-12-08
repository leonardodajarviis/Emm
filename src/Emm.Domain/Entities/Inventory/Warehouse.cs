using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Inventory;

/// <summary>
/// Thực thể đại diện cho Kho hàng trong hệ thống quản lý kho.
/// </summary>
public class Warehouse : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public long? CreatedByUserId { get; private set; }
    public long? UpdatedByUserId { get; private set; }

    private Warehouse() { }

    public Warehouse(string code, string name, string? address = null, string? description = null)
    {
        Code = code;
        Name = name;
        Address = address;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string? address, string? description)
    {
        Name = name;
        Address = address;
        Description = description;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
