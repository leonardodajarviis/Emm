using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record DeactivateRoleCommand(
    long Id
) : IRequest<Result<object>>;
