using Emm.Domain.Entities.Authorization;

namespace Emm.Application.Abstractions;

/// <summary>
/// Evaluates ABAC policies
/// </summary>
public interface IPolicyEvaluator
{
    /// <summary>
    /// Evaluate if a policy allows access
    /// </summary>
    Task<bool> EvaluateAsync(
        Policy policy,
        PolicyContext context,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Evaluate all applicable policies for a request
    /// </summary>
    Task<bool> EvaluatePoliciesAsync(
        IEnumerable<Policy> policies,
        PolicyContext context,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Context for policy evaluation
/// </summary>
public class PolicyContext
{
    public Guid UserId { get; set; }
    public Guid? UserOrganizationUnitId { get; set; }
    public string Resource { get; set; } = null!;
    public string Action { get; set; } = null!;
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> ResourceAttributes { get; set; } = new();

    public T? GetResourceAttribute<T>(string key)
    {
        if (ResourceAttributes.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;
        return default;
    }
}
