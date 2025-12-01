namespace Emm.Application.Features.AppOrganizationUnit.Dtos;

public record UpdateOrganizationUnit
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long OrganizationUnitLevelId { get; set; }
    public bool IsActive { get; set; } = true;
    public long? ParentId { get; set; }
}
