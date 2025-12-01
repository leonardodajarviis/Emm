using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Emm.Presentation.Authorization;

/// <summary>
/// Attribute for ABAC authorization with resource context
/// Usage: [AuthorizeResource("OperationShift", "Create")]
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AuthorizeResourceAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _resource;
    private readonly string _action;
    private readonly string? _resourceContextParameter;

    /// <summary>
    /// Authorize access to resource with action
    /// </summary>
    /// <param name="resource">Resource type</param>
    /// <param name="action">Action on resource</param>
    /// <param name="resourceContextParameter">Optional: parameter name containing resource context</param>
    public AuthorizeResourceAttribute(string resource, string action, string? resourceContextParameter = null)
    {
        _resource = resource ?? throw new ArgumentNullException(nameof(resource));
        _action = action ?? throw new ArgumentNullException(nameof(action));
        _resourceContextParameter = resourceContextParameter;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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

        // Get resource context if parameter name specified
        object? resourceContext = null;
        if (!string.IsNullOrEmpty(_resourceContextParameter))
        {
            if (context.ActionArguments.TryGetValue(_resourceContextParameter, out var contextValue))
            {
                resourceContext = contextValue;
            }
        }

        // Check access with ABAC policies
        var canAccess = await authService.CanAccessAsync(userId, _resource, _action, resourceContext);

        if (!canAccess)
        {
            context.Result = new ForbidResult();
            return;
        }

        await next();
    }
}
