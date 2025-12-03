using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record ActivateRoleCommand(
    long Id
) : IRequest<Result<object>>;
