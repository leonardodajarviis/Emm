using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record UpdateRoleCommand(
    long Id,
    string Name,
    string? Description
) : IRequest<Result<object>>;
