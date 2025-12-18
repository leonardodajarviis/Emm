using Emm.Application.Features.AppUser.Commands;
using Emm.Application.Features.AppUser.Dtos;
using Emm.Application.Features.AppUser.Queries;
using Emm.Application.Common;
using Emm.Presentation.Extensions;
using LazyNet.Symphony.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emm.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUser createUser)
    {
        var command = new CreateUserCommand(
            Username: createUser.Username,
            DisplayName: createUser.DisplayName,
            Password: createUser.Password,
            Email: createUser.Email
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUser updateUser)
    {
        var command = new UpdateUserCommand(
            Id: id,
            Username: updateUser.Username,
            Email: updateUser.Email
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById([FromRoute] Guid id)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] QueryParam QueryRequest)
    {
        var query = new GetUsersQuery(QueryRequest);
        var result = await _mediator.Send(query);
        return result.ToActionResult();
    }

    [HttpPut("{id}/change-password")]
    public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePassword changePassword)
    {
        var command = new ChangePasswordCommand(
            Id: id,
            CurrentPassword: changePassword.CurrentPassword,
            NewPassword: changePassword.NewPassword
        );

        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id}/activate")]
    public async Task<IActionResult> ActivateUser([FromRoute] Guid id)
    {
        var command = new ActivateUserCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpPut("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser([FromRoute] Guid id)
    {
        var command = new DeactivateUserCommand(id);
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }
}
