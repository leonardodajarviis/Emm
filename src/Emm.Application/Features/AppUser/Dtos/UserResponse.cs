namespace Emm.Application.Features.AppUser.Dtos;

public record UserResponse
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
