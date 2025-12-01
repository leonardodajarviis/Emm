using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record ActivateUserCommand(
    long Id
) : IRequest<Result<object>>;
