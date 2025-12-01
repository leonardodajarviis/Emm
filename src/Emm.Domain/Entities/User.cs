using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities;

public class User : AggregateRoot
{
    public long Id { get; private set; }
    public string Username { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public long? EmployeeId { get; private set; }
    public long? OrganizationUnitId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public User(string username, string displayName, string passwordHash, string email)
    {
        DisplayName = displayName;
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string username, string email)
    {
        Username = username;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmployeeId(long employeeId)
    {
        EmployeeId = employeeId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOrganizationUnitId(long organizationUnitId)
    {
        OrganizationUnitId = organizationUnitId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }


    public User() { }
}