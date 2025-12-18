using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record UpdateRoleCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest<Result<object>>;
