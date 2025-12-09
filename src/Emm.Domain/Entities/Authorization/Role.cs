using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// Role - Vai trò để gom nhóm các permissions
/// VD: Admin, Shift Supervisor, Maintenance Technician
/// </summary>
public class Role : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }

    /// <summary>
    /// Unique role code (VD: ADMIN, SHIFT_SUPERVISOR)
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Display name
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// System roles cannot be deleted
    /// </summary>
    public bool IsSystemRole { get; private set; }

    /// <summary>
    /// Active status
    /// </summary>
    public bool IsActive { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    // Navigation properties
    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    public Role(
        string code,
        string name,
        string? description = null,
        bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Role code cannot be empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Description = description?.Trim();
        IsSystemRole = isSystemRole;
        IsActive = true;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        if (IsSystemRole)
            throw new InvalidOperationException("Cannot deactivate system role");

        IsActive = false;
    }

    public void AddPermission(long permissionId)
    {
        if (_rolePermissions.Any(rp => rp.PermissionId == permissionId))
            return; // Already exists

        _rolePermissions.Add(new RolePermission(Id, permissionId));
    }

    public void RemovePermission(long permissionId)
    {
        var rolePermission = _rolePermissions.FirstOrDefault(rp => rp.PermissionId == permissionId);
        if (rolePermission != null)
        {
            _rolePermissions.Remove(rolePermission);
        }
    }

    private Role() { } // EF Core constructor
}
