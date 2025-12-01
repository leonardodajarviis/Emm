namespace Emm.Application.Features.AppLocation.Dtos;

public class CreateLocation
{
    public required string Name { get; set; }
    public required long OrganizationUnitId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
