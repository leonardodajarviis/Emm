using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// Permission - Quyền hạn cơ bản (Resource.Action)
/// VD: "OperationShift.Create", "Asset.View", "Report.Export"
/// </summary>
public class Permission : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }

    /// <summary>
    /// Resource type (VD: OperationShift, Asset, User)
    /// </summary>
    public string Resource { get; private set; } = null!;

    /// <summary>
    /// Action on resource (VD: Create, Read, Update, Delete, Export)
    /// </summary>
    public string Action { get; private set; } = null!;

    /// <summary>
    /// Full permission code: Resource.Action (VD: OperationShift.Create)
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Display name for UI
    /// </summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Category for grouping (VD: Operations, Assets, Reports)
    /// </summary>
    public string? Category { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions;

    private readonly List<UserPermission> _userPermissions = [];
    public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions;

    public Permission(
        string resource,
        string action,
        string displayName,
        string? description = null,
        string? category = null)
    {
        if (string.IsNullOrWhiteSpace(resource))
            throw new ArgumentException("Resource cannot be empty", nameof(resource));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        Resource = resource.Trim();
        Action = action.Trim();
        Code = $"{Resource}.{Action}";
        DisplayName = displayName.Trim();
        Description = description?.Trim();
        Category = category?.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string displayName, string? description, string? category)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name cannot be empty", nameof(displayName));

        DisplayName = displayName.Trim();
        Description = description?.Trim();
        Category = category?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    private Permission() { } // EF Core constructor
}
