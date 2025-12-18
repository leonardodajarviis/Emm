using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record ActivateRoleCommand(
    Guid Id
) : IRequest<Result<object>>;
