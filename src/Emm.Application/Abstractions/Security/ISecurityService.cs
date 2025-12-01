namespace Emm.Application.Abstractions.Security;

public interface ISecurityService
{
    /// <summary>
    /// Checks if the current user can perform the specified action on the given resource instance (ABAC).
    /// </summary>
    Task<bool> AuthorizeAsync<TResource>(TResource resource, ResourceAction action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user has the general permission to perform the action on the resource type (RBAC).
    /// </summary>
    Task<bool> AuthorizeAsync(string resourceName, ResourceAction action, CancellationToken cancellationToken = default);
}
