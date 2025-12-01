using System.Security.Claims;

namespace Emm.Application.Abstractions;

public interface IUserContextService
{
    long? GetCurrentUserId();
    string? GetCurrentUsername();
    long? GetCurrentEmployeeId();
    long? GetCurrentOrganizationUnitId();
    string? GetCurrentEmail();
    ClaimsPrincipal? GetCurrentUser();
    bool IsAuthenticated();
    string? GetAccessToken();
}
