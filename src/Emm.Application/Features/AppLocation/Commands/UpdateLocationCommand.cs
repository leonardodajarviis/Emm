namespace Emm.Application.Features.AppLocation.Commands;

public record UpdateLocationCommand(
    long Id,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<Result<object>>;
