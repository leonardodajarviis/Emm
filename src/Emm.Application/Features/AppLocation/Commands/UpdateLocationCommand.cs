namespace Emm.Application.Features.AppLocation.Commands;

public record UpdateLocationCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<Result<object>>;
