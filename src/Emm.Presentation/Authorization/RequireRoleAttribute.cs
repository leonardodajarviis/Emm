using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Emm.Presentation.Authorization;

/// <summary>
/// Attribute to require specific roles for controller actions
/// Usage: [RequireRole("ADMIN", "MANAGER")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _roles;
    private readonly bool _requireAll;

    /// <summary>
    /// Require role(s)
    /// </summary>
    /// <param name="roles">Role codes</param>
    public RequireRoleAttribute(params string[] roles)
    {
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
        _requireAll = false;
    }

    /// <summary>
    /// Require role(s) with option to require all or any
    /// </summary>
    /// <param name="requireAll">If true, user must have all roles. If false, any role is sufficient.</param>
    /// <param name="roles">Role codes</param>
    public RequireRoleAttribute(bool requireAll, params string[] roles)
    {
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
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

        // Check roles
        var userRoles = await authService.GetUserRolesAsync(userId);

        bool hasRole;
        if (_requireAll)
        {
            hasRole = _roles.All(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
        }
        else
        {
            hasRole = _roles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
        }

        if (!hasRole)
        {
            context.Result = new ForbidResult();
        }
    }
}
