namespace Emm.Application.Features.AppRole.Dtos;

public record UpdateRole
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}
