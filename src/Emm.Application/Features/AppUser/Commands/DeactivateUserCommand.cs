using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record DeactivateUserCommand(
    long Id
) : IRequest<Result<object>>;
