namespace Emm.Application.Features.AppUser.Dtos;

public record ChangePassword
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
