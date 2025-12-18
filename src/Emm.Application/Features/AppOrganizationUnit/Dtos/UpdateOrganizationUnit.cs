namespace Emm.Application.Features.AppOrganizationUnit.Dtos;

public record UpdateOrganizationUnit
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Guid OrganizationUnitLevelId { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentId { get; set; }
}
