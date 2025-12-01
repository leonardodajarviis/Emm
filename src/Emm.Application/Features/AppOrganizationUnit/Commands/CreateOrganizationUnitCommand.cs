namespace Emm.Application.Features.AppOrganizationUnit.Commands;

public record CreateOrganizationUnitCommand(
    string Name,
    long OrganizationUnitLevelId,
    string? Description = null,
    bool IsActive = true,
    long? ParentId = null
) : IRequest<Result<object>>;
