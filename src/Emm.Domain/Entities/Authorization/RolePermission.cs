namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// Many-to-many relationship between Role and Permission
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }
    public DateTime GrantedAt { get; private set; }

    // Navigation properties
    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    public RolePermission(Guid roleId, Guid permissionId)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        GrantedAt = DateTime.UtcNow;
    }

    private RolePermission() { } // EF Core constructor
}
