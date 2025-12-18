namespace Emm.Application.Features.AppOrganizationUnitLevel.Commands;

public record UpdateOrganizationUnitLevelCommand(
    Guid Id,
    string Name,
    string? Description,
    int Level
) : IRequest<Result<object>>;
