namespace Emm.Application.Features.AppOrganizationUnit.Dtos;

public class CreateOrganizationUnit
{
    public required string Name { get; set; }
    public required long OrganizationUnitLevelId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public long? ParentId { get; set; }
}
