using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record DeleteUserCommand(
    long Id
) : IRequest<Result<object>>;
