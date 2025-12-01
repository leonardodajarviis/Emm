namespace Emm.Application.Features.AppEmployee.Dtos;

public record UpdateEmployee
{
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
}
