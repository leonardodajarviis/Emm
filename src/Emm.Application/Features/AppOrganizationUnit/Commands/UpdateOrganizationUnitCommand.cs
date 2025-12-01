namespace Emm.Application.Features.AppOrganizationUnit.Commands;

public record UpdateOrganizationUnitCommand(
    long Id,
    string Name,
    long OrganizationUnitLevelId,
    string? Description,
    bool IsActive,
    long? ParentId
) : IRequest<Result<object>>;
