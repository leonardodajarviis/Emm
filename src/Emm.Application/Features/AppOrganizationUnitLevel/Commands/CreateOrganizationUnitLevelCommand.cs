namespace Emm.Application.Features.AppOrganizationUnitLevel.Commands;

public record CreateOrganizationUnitLevelCommand(
    string Name,
    int Level,
    string? Description = null
) : IRequest<Result<object>>;
