namespace Emm.Application.Features.AppLocation.Dtos;

public record UpdateLocation
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
