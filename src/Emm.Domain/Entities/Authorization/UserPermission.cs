namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// Direct permission assignment to User (override role permissions)
/// </summary>
public class UserPermission
{
    public long UserId { get; private set; }
    public long PermissionId { get; private set; }

    /// <summary>
    /// Grant (true) or Deny (false) permission
    /// Deny takes precedence over Grant
    /// </summary>
    public bool IsGranted { get; private set; }

    public DateTime AssignedAt { get; private set; }
    public long? AssignedBy { get; private set; }
    public string? Reason { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    public UserPermission(
        long userId,
        long permissionId,
        bool isGranted = true,
        long? assignedBy = null,
        string? reason = null)
    {
        UserId = userId;
        PermissionId = permissionId;
        IsGranted = isGranted;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
        Reason = reason?.Trim();
    }

    private UserPermission() { } // EF Core constructor
}
