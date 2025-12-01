using System.Security.Claims;
using Emm.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Emm.Infrastructure.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? GetCurrentUserId()
    {
        var userIdClaim = GetCurrentUser()?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return long.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public string? GetCurrentUsername()
    {
        return GetCurrentUser()?.FindFirst(ClaimTypes.Name)?.Value;
    }

    public long? GetCurrentEmployeeId()
    {
        var employeeIdClaim = GetCurrentUser()?.FindFirst("employeeId")?.Value;
        if (employeeIdClaim == "no-employee")
            return null;
        return long.TryParse(employeeIdClaim, out var employeeId) ? employeeId : null;
    }

    public long? GetCurrentOrganizationUnitId()
    {
        var orgUnitIdClaim = GetCurrentUser()?.FindFirst("organizationUnitId")?.Value;
        if (orgUnitIdClaim == "no-organization-unit")
            return null;
        return long.TryParse(orgUnitIdClaim, out var orgUnitId) ? orgUnitId : null;
    }

    public string? GetCurrentEmail()
    {
        return GetCurrentUser()?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }

    public bool IsAuthenticated()
    {
        return GetCurrentUser()?.Identity?.IsAuthenticated == true;
    }

    public string? GetAccessToken()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return null;

        // Try to get token from Authorization header
        var authHeader = request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader.Substring("Bearer ".Length).Trim();
        }

        return null;
    }
}
