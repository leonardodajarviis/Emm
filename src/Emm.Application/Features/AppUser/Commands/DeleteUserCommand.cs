using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record DeleteUserCommand(
    Guid Id
) : IRequest<Result<object>>;
