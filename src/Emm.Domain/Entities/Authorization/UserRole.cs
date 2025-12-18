namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// Many-to-many relationship between User and Role
/// </summary>
public class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    public UserRole(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }

    private UserRole() { } // EF Core constructor
}
