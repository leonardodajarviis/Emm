namespace Emm.Application.Features.AppOrganizationUnit.Dtos;

public record OrganizationUnitResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public long? ParentId { get; set; }
    public required long OrganizationUnitLevelId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
