namespace Emm.Application.Features.AppAuth.Dtos;


public record UserAuthResponse(
    string AccessToken,
    string RefreshToken,
    long AccessTokenExpiresAt,
    long RefreshTokenExpiresAt,
    UserAuthInfo User
    );

public record UserAuthInfo(
    Guid UserId,
    string Username,
    string DisplayName,
    string Email);

