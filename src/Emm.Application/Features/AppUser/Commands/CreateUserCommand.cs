using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record CreateUserCommand(
    string Username,
    string DisplayName,
    string Password,
    string Email
) : IRequest<Result<object>>;
