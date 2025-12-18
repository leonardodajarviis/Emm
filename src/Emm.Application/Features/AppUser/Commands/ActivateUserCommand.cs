using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record ActivateUserCommand(
    Guid Id
) : IRequest<Result<object>>;
