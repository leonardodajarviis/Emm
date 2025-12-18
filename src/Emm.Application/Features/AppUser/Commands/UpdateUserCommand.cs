using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record UpdateUserCommand(
    Guid Id,
    string Username,
    string Email
) : IRequest<Result<object>>;
