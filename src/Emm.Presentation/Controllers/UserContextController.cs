using Emm.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserContextController : ControllerBase
{
    private readonly IUserContextService _userContextService;

    public UserContextController(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    [HttpGet("current")]
    public IActionResult GetCurrentUserContext()
    {
        if (!_userContextService.IsAuthenticated())
        {
            return Unauthorized("User is not authenticated");
        }

        var userContext = new
        {
            UserId = _userContextService.GetCurrentUserId(),
            Username = _userContextService.GetCurrentUsername(),
            Email = _userContextService.GetCurrentEmail(),
            EmployeeId = _userContextService.GetCurrentEmployeeId(),
            OrganizationUnitId = _userContextService.GetCurrentOrganizationUnitId(),
            IsAuthenticated = _userContextService.IsAuthenticated()
        };

        return Ok(userContext);
    }

    [HttpGet("claims")]
    public IActionResult GetAllClaims()
    {
        if (!_userContextService.IsAuthenticated())
        {
            return Unauthorized("User is not authenticated");
        }

        var user = _userContextService.GetCurrentUser();
        var claims = user?.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(claims);
    }
}
