namespace Emm.Application.Features.AppOrganizationUnitLevel.Commands;

public record UpdateOrganizationUnitLevelCommand(
    long Id,
    string Name,
    string? Description,
    int Level
) : IRequest<Result<object>>;
