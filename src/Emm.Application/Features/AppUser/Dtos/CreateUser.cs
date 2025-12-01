namespace Emm.Application.Features.AppUser.Dtos;

public record CreateUser
{
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
}
