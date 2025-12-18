namespace Emm.Application.Features.AppOrganizationUnitLevel.Dtos;

public class CreateOrganizationUnitLevel
{
    public required string Name { get; set; }
    public required int Level { get; set; }
    public string? Description { get; set; }
}

public record UpdateOrganizationUnitLevel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required int Level { get; set; }
}

public record OrganizationUnitLevelResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required int Level { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
