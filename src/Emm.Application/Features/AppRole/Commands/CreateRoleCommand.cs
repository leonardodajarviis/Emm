using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppRole.Commands;

public record CreateRoleCommand(
    string Code,
    string Name,
    string? Description,
    bool IsSystemRole = false
) : IRequest<Result<object>>;
