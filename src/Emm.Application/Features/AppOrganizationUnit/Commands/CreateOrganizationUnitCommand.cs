namespace Emm.Application.Features.AppOrganizationUnit.Commands;

public record CreateOrganizationUnitCommand(
    string Name,
    Guid OrganizationUnitLevelId,
    string? Description = null,
    bool IsActive = true,
    Guid? ParentId = null
) : IRequest<Result<object>>;
