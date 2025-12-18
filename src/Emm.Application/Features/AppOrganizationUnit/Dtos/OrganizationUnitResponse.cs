namespace Emm.Application.Features.AppOrganizationUnit.Dtos;

public record OrganizationUnitResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public required Guid OrganizationUnitLevelId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
