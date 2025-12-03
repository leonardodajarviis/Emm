namespace Emm.Application.Features.AppRole.Dtos;

public record CreateRole
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsSystemRole { get; init; }
}
