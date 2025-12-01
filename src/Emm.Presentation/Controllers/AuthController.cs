using Emm.Application.Features.AppAuth.Commands;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Emm.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    // [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var command = new UserLoginCommand(
            request.Username,
            request.Password,
            ipAddress,
            userAgent
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var command = new UserLogoutCommand();
        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}

public record UserLoginRequest(string Username, string Password);

public record RefreshTokenRequest(string RefreshToken);
