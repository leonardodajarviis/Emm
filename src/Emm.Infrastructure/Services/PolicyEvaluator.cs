using Emm.Application.Abstractions;
using Emm.Domain.Entities.Authorization;
using System.Text.Json;

namespace Emm.Infrastructure.Services;

/// <summary>
/// Evaluates ABAC policies based on policy type and conditions
/// </summary>
public class PolicyEvaluator : IPolicyEvaluator
{
    public Task<bool> EvaluateAsync(
        Policy policy,
        PolicyContext context,
        CancellationToken cancellationToken = default)
    {
        if (!policy.IsActive)
            return Task.FromResult(false);

        return policy.Type switch
        {
            PolicyType.OrganizationUnit => EvaluateOrganizationUnitPolicy(policy, context),
            PolicyType.Time => EvaluateTimePolicy(policy, context),
            PolicyType.ResourceAttribute => EvaluateResourceAttributePolicy(policy, context),
            PolicyType.Custom => EvaluateCustomPolicy(policy, context),
            _ => Task.FromResult(true)
        };
    }

    public async Task<bool> EvaluatePoliciesAsync(
        IEnumerable<Policy> policies,
        PolicyContext context,
        CancellationToken cancellationToken = default)
    {
        var applicablePolicies = policies
            .Where(p => p.IsActive)
            .Where(p => IsApplicable(p, context))
            .OrderByDescending(p => p.Priority)
            .ToList();

        if (!applicablePolicies.Any())
            return true; // No policies = allow

        // All policies must pass
        foreach (var policy in applicablePolicies)
        {
            var result = await EvaluateAsync(policy, context, cancellationToken);
            if (!result)
                return false;
        }

        return true;
    }

    private bool IsApplicable(Policy policy, PolicyContext context)
    {
        // Check if policy applies to this resource/action
        if (policy.ResourceType != null && policy.ResourceType != context.Resource)
            return false;

        if (policy.Action != null && policy.Action != context.Action)
            return false;

        return true;
    }

    private Task<bool> EvaluateOrganizationUnitPolicy(Policy policy, PolicyContext context)
    {
        var conditions = policy.GetConditions<OrganizationUnitConditions>();
        if (conditions == null)
            return Task.FromResult(true);

        // User must have OrganizationUnitId
        if (!context.UserOrganizationUnitId.HasValue)
            return Task.FromResult(false);

        // Check if resource has OrganizationUnitId attribute
        var resourceOrgUnitId = context.GetResourceAttribute<long?>("OrganizationUnitId");

        if (conditions.RequireSameOrganizationUnit)
        {
            // Resource must belong to same organization unit as user
            if (resourceOrgUnitId.HasValue && resourceOrgUnitId.Value != context.UserOrganizationUnitId.Value)
                return Task.FromResult(false);
        }

        if (conditions.AllowedOrganizationUnitIds?.Any() == true)
        {
            // User must be in allowed organization units
            if (!conditions.AllowedOrganizationUnitIds.Contains(context.UserOrganizationUnitId.Value))
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private Task<bool> EvaluateTimePolicy(Policy policy, PolicyContext context)
    {
        var conditions = policy.GetConditions<TimeConditions>();
        if (conditions == null)
            return Task.FromResult(true);

        var now = context.RequestTime;

        // Check working hours
        if (conditions.WorkingHoursOnly)
        {
            var startHour = conditions.StartHour ?? 8;
            var endHour = conditions.EndHour ?? 18;

            if (now.Hour < startHour || now.Hour >= endHour)
                return Task.FromResult(false);
        }

        // Check allowed days of week
        if (conditions.AllowedDaysOfWeek?.Any() == true)
        {
            if (!conditions.AllowedDaysOfWeek.Contains((int)now.DayOfWeek))
                return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private Task<bool> EvaluateResourceAttributePolicy(Policy policy, PolicyContext context)
    {
        var conditions = policy.GetConditions<ResourceAttributeConditions>();
        if (conditions == null)
            return Task.FromResult(true);

        // Check required attributes
        if (conditions.RequiredAttributes?.Any() == true)
        {
            foreach (var attr in conditions.RequiredAttributes)
            {
                if (!context.ResourceAttributes.ContainsKey(attr.Key))
                    return Task.FromResult(false);

                var value = context.ResourceAttributes[attr.Key];
                if (value?.ToString() != attr.Value)
                    return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }

    private Task<bool> EvaluateCustomPolicy(Policy policy, PolicyContext context)
    {
        // For custom policies, implement specific logic based on conditions
        // This is a placeholder - implement based on your needs
        return Task.FromResult(true);
    }
}

// Policy condition models
public class OrganizationUnitConditions
{
    public bool RequireSameOrganizationUnit { get; set; }
    public List<long>? AllowedOrganizationUnitIds { get; set; }
}

public class TimeConditions
{
    public bool WorkingHoursOnly { get; set; }
    public int? StartHour { get; set; }
    public int? EndHour { get; set; }
    public List<int>? AllowedDaysOfWeek { get; set; }
}

public class ResourceAttributeConditions
{
    public Dictionary<string, string>? RequiredAttributes { get; set; }
}
