using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record DeleteRoleCommand(
    long Id
) : IRequest<Result<object>>;
