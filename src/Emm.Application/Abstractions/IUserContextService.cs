using System.Security.Claims;

namespace Emm.Application.Abstractions;

public interface IUserContextService
{
    Guid? GetCurrentUserId();
    string? GetCurrentUsername();
    Guid? GetCurrentOrganizationUnitId();
    string? GetCurrentEmail();
    ClaimsPrincipal? GetCurrentUser();
    bool IsAuthenticated();
    string? GetAccessToken();
}
