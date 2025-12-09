using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;
using System.Text.Json;

namespace Emm.Domain.Entities.Authorization;

/// <summary>
/// ABAC Policy - Attribute-based access control rules
/// VD: OrganizationUnit-based, Time-based, Resource-based policies
/// </summary>
public class Policy : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }

    /// <summary>
    /// Policy code (VD: ORG_UNIT_RESTRICTION, WORKING_HOURS_ONLY)
    /// </summary>
    public string Code { get; private set; } = null!;

    /// <summary>
    /// Policy name
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Resource type this policy applies to (VD: OperationShift, Asset)
    /// Null = applies to all resources
    /// </summary>
    public string? ResourceType { get; private set; }

    /// <summary>
    /// Action this policy applies to (VD: Create, Update, Delete)
    /// Null = applies to all actions
    /// </summary>
    public string? Action { get; private set; }

    /// <summary>
    /// Policy type (OrganizationUnit, Time, Custom)
    /// </summary>
    public PolicyType Type { get; private set; }

    /// <summary>
    /// JSON conditions for policy evaluation
    /// </summary>
    public string ConditionsJson { get; private set; } = "{}";

    /// <summary>
    /// Priority (higher = evaluated first)
    /// </summary>
    public int Priority { get; private set; }

    /// <summary>
    /// Active status
    /// </summary>
    public bool IsActive { get; private set; }

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    public Policy(
        string code,
        string name,
        PolicyType type,
        string? resourceType = null,
        string? action = null,
        string? description = null,
        int priority = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Policy code cannot be empty", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Policy name cannot be empty", nameof(name));

        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Type = type;
        ResourceType = resourceType?.Trim();
        Action = action?.Trim();
        Description = description?.Trim();
        Priority = priority;
        IsActive = true;
    }

    public void Update(string name, string? description, int priority)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Policy name cannot be empty", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        Priority = priority;
    }

    public void SetConditions(object conditions)
    {
        ConditionsJson = JsonSerializer.Serialize(conditions);
    }

    public T? GetConditions<T>() where T : class
    {
        if (string.IsNullOrWhiteSpace(ConditionsJson) || ConditionsJson == "{}")
            return null;

        return JsonSerializer.Deserialize<T>(ConditionsJson);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private Policy() { } // EF Core constructor
}

public enum PolicyType
{
    /// <summary>
    /// Restricts access based on OrganizationUnit
    /// </summary>
    OrganizationUnit = 0,

    /// <summary>
    /// Restricts access based on time (working hours, shifts)
    /// </summary>
    Time = 1,

    /// <summary>
    /// Restricts access based on resource attributes
    /// </summary>
    ResourceAttribute = 2,

    /// <summary>
    /// Custom policy with complex conditions
    /// </summary>
    Custom = 99
}
