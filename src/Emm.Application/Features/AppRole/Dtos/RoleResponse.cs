namespace Emm.Application.Features.AppRole.Dtos;

public record RoleResponse
{
    public required long Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required bool IsSystemRole { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
