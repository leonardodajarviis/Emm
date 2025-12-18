namespace Emm.Application.Features.AppOrganizationUnit.Commands;

public record UpdateOrganizationUnitCommand(
    Guid Id,
    string Name,
    Guid OrganizationUnitLevelId,
    string? Description,
    bool IsActive,
    Guid? ParentId
) : IRequest<Result<object>>;
