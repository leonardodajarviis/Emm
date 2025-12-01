namespace Emm.Application.Features.AppAuth.Dtos;


public record UserAuthResponse(
    string AccessToken,
    string RefreshToken,
    long AccessTokenExpiresAt,
    long RefreshTokenExpiresAt,
    UserAuthInfo User
    );

public record UserAuthInfo(
    long UserId,
    string Username,
    string DisplayName,
    string Email);

