using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Emm.Presentation.Authorization;

/// <summary>
/// Attribute to require specific permissions for controller actions
/// Usage: [RequirePermission("OperationShift.Create")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _permissions;
    private readonly bool _requireAll;

    /// <summary>
    /// Require permission(s)
    /// </summary>
    /// <param name="permissions">Permission codes (e.g., "OperationShift.Create")</param>
    public RequirePermissionAttribute(params string[] permissions)
    {
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        _requireAll = false;
    }

    /// <summary>
    /// Require permission(s) with option to require all or any
    /// </summary>
    /// <param name="requireAll">If true, user must have all permissions. If false, any permission is sufficient.</param>
    /// <param name="permissions">Permission codes</param>
    public RequirePermissionAttribute(bool requireAll, params string[] permissions)
    {
        _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        _requireAll = requireAll;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authService = context.HttpContext.RequestServices
            .GetService(typeof(Application.Abstractions.IAuthorizationService))
            as Application.Abstractions.IAuthorizationService;

        if (authService == null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            return;
        }

        // Get user ID from claims
        var userIdClaim = context.HttpContext.User.FindFirst("userId");
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check permissions
        bool hasPermission;
        if (_requireAll)
        {
            hasPermission = await authService.HasAllPermissionsAsync(userId, _permissions);
        }
        else
        {
            hasPermission = await authService.HasAnyPermissionAsync(userId, _permissions);
        }

        if (!hasPermission)
        {
            context.Result = new ForbidResult();
        }
    }
}
