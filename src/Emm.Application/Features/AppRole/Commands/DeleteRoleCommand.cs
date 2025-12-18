using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record DeleteRoleCommand(
    Guid Id
) : IRequest<Result<object>>;
