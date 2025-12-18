namespace Emm.Application.Abstractions;

/// <summary>
/// Authorization service for checking permissions and policies
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Check if user has a specific permission
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has any of the specified permissions
    /// </summary>
    Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissionCodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has all of the specified permissions
    /// </summary>
    Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissionCodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user can perform action on resource (with ABAC policy evaluation)
    /// </summary>
    Task<bool> CanAccessAsync(
        Guid userId,
        string resource,
        string action,
        object? resourceContext = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all permission codes for a user
    /// </summary>
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all roles for a user
    /// </summary>
    Task<IReadOnlyList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
