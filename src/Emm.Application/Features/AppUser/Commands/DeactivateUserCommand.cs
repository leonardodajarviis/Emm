using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppUser.Commands;

public record DeactivateUserCommand(
    Guid Id
) : IRequest<Result<object>>;
