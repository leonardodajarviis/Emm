namespace Emm.Application.Features.AppLocation.Dtos;

public record LocationResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long OrganizationUnitId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
