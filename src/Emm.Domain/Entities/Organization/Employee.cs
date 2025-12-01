using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Organization;

public class Employee : AggregateRoot
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string? LastName { get; private set; }
    public long? OrganizationUnitId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Employee(string code,string displayName, string firstName, string? lastName, long? organizationId)
    {
        Code = code;
        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        OrganizationUnitId = organizationId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string displayName, string firstName, string? lastName)
    {
        DisplayName = displayName;
        FirstName = firstName;
        LastName = lastName;
        UpdatedAt = DateTime.UtcNow;
    }

    public Employee() { }
}