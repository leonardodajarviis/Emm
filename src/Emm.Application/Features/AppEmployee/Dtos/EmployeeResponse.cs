namespace Emm.Application.Features.AppEmployee.Dtos;

public record EmployeeResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public long? OrganizationUnitId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    
    // Related data
    public OrganizationUnitResponse? OrganizationUnit { get; set; }
}

public record OrganizationUnitResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}
