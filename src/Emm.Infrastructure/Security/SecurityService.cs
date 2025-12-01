using Emm.Application.Abstractions.Security;

namespace Emm.Infrastructure.Security;

public class SecurityService : ISecurityService
{
    private readonly IServiceProvider _serviceProvider;

    public SecurityService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> AuthorizeAsync<TResource>(TResource resource, ResourceAction action, CancellationToken cancellationToken = default)
    {
        if (resource == null) return false;

        // Dynamic dispatch to find the correct policy
        var policyType = typeof(IResourcePolicy<>).MakeGenericType(resource.GetType());
        var policy = _serviceProvider.GetService(policyType);

        if (policy == null)
        {
            // Fallback: If no specific policy exists, default to Allow or Deny?
            // For security, usually Default Deny.
            // For development speed, maybe Default Allow (use with caution).
            return true;
        }

        // Invoke CheckAsync method via reflection (or dynamic)
        var method = policyType.GetMethod("CheckAsync");
        if (method == null) return false;

        var task = (Task<bool>)method.Invoke(policy, new object[] { resource, action, cancellationToken })!;
        return await task;
    }

    public Task<bool> AuthorizeAsync(string resourceName, ResourceAction action, CancellationToken cancellationToken = default)
    {
        // TODO: Implement RBAC checks here (e.g., check Permission table in DB)
        return Task.FromResult(true);
    }
}
