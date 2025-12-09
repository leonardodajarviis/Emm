using Emm.Application.Abstractions;
using Emm.Application.Authorization;
using Emm.Application.Common;
using LazyNet.Symphony.Core;
using Microsoft.Extensions.Logging;

namespace Emm.Application.Behaviors;

/// <summary>
/// Pipeline behavior for automatic authorization checking
/// </summary>
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserContextService _userContextService;

    public AuthorizationBehavior(
        IAuthorizationService authorizationService,
        IUserContextService userContextService)
    {
        _authorizationService = authorizationService;
        _userContextService = userContextService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        PipelineNext<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        // Skip authorization for public requests (Login, Register, etc.)
        if (request is IPublicRequest)
        {
            return await next();
        }

        // Get current user ID
        var userId = _userContextService.GetCurrentUserId();
        if (userId == null)
        {
            return AuthorizationBehavior<TRequest, TResponse>.CreateUnauthorizedResult();
        }

        // 1. Check IRequirePermission
        if (request is IRequirePermission permissionRequest)
        {
            var hasPermission = permissionRequest.RequireAll
                ? await _authorizationService.HasAllPermissionsAsync(userId.Value, permissionRequest.RequiredPermissions, cancellationToken)
                : await _authorizationService.HasAnyPermissionAsync(userId.Value, permissionRequest.RequiredPermissions, cancellationToken);

            if (!hasPermission)
            {
                return CreateForbiddenResult($"Missing required permission(s): {string.Join(", ", permissionRequest.RequiredPermissions)}");
            }
        }

        // 2. Check IRequireRole
        if (request is IRequireRole roleRequest)
        {
            var userRoles = await _authorizationService.GetUserRolesAsync(userId.Value, cancellationToken);

            var hasRole = roleRequest.RequireAll
                ? roleRequest.RequiredRoles.All(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase))
                : roleRequest.RequiredRoles.Any(r => userRoles.Contains(r, StringComparer.OrdinalIgnoreCase));

            if (!hasRole)
            {
                return CreateForbiddenResult($"Missing required role(s): {string.Join(", ", roleRequest.RequiredRoles)}");
            }
        }

        // 3. Check IRequireResourceAccess (ABAC)
        if (request is IRequireResourceAccess resourceRequest)
        {
            var canAccess = await _authorizationService.CanAccessAsync(
                userId.Value,
                resourceRequest.Resource,
                resourceRequest.Action,
                resourceRequest.GetResourceContext(),
                cancellationToken);

            if (!canAccess)
            {
                return CreateForbiddenResult($"Access denied to {resourceRequest.Resource}.{resourceRequest.Action}");
            }
        }

        // All checks passed - proceed to handler
        return await next();
    }

    private static TResponse CreateUnauthorizedResult()
    {
        // Check if TResponse is Result or Result<T>
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Unauthorized("User is not authenticated", "AUTHORIZATION_UNAUTHENTICATED");
        }

        // For Result<T>, use reflection to call static Unauthorized method
        var resultType = typeof(TResponse);
        var method = resultType.GetMethod("Unauthorized", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (method != null)
        {
            var result = method.Invoke(null, new object?[] { "User is not authenticated", "AUTHORIZATION_UNAUTHENTICATED" });
            return (TResponse)result!;
        }

        throw new InvalidOperationException($"Cannot create unauthorized result for type {typeof(TResponse).Name}");
    }

    private TResponse CreateForbiddenResult(string message)
    {
        // Check if TResponse is Result or Result<T>
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Forbidden(message, "AUTHORIZATION_FORBIDDEN");
        }

        // For Result<T>, use reflection to call static Forbidden method
        var resultType = typeof(TResponse);
        var method = resultType.GetMethod("Forbidden", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        if (method != null)
        {
            var result = method.Invoke(null, new object?[] { message, "AUTHORIZATION_FORBIDDEN" });
            return (TResponse)result!;
        }

        throw new InvalidOperationException($"Cannot create forbidden result for type {typeof(TResponse).Name}");
    }
}
