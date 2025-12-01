namespace Emm.Application.Authorization;

/// <summary>
/// Marker interface for commands/queries that require specific permission(s)
/// </summary>
public interface IRequirePermission
{
    /// <summary>
    /// Permission codes required (ANY logic by default)
    /// </summary>
    string[] RequiredPermissions { get; }

    /// <summary>
    /// If true, user must have ALL permissions. If false, ANY permission is sufficient.
    /// </summary>
    bool RequireAll => false;
}

/// <summary>
/// Marker interface for commands/queries that require specific role(s)
/// </summary>
public interface IRequireRole
{
    /// <summary>
    /// Role codes required (ANY logic by default)
    /// </summary>
    string[] RequiredRoles { get; }

    /// <summary>
    /// If true, user must have ALL roles. If false, ANY role is sufficient.
    /// </summary>
    bool RequireAll => false;
}

/// <summary>
/// Marker interface for commands/queries that need ABAC authorization
/// </summary>
public interface IRequireResourceAccess
{
    /// <summary>
    /// Resource type
    /// </summary>
    string Resource { get; }

    /// <summary>
    /// Action on resource
    /// </summary>
    string Action { get; }

    /// <summary>
    /// Resource context for ABAC policy evaluation
    /// </summary>
    object? GetResourceContext() => null;
}
