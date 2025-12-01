namespace Emm.Application.Features.AppLocation.Commands;

public record CreateLocationCommand(
    string Name,
    long OrganizationUnitId,
    string? Description = null,
    bool IsActive = true
) : IRequest<Result<object>>;
