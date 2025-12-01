namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// Many-to-many relationship between User and Role
/// </summary>
public class UserRole
{
    public long UserId { get; private set; }
    public long RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public long? AssignedBy { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    public UserRole(long userId, long roleId, long? assignedBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }

    private UserRole() { } // EF Core constructor
}
