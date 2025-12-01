namespace Emm.Application.Features.AppUser.Dtos;

public record UpdateUser
{
    public required string Username { get; set; }
    public required string Email { get; set; }
}
