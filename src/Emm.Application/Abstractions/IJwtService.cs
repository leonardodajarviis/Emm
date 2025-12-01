using System.Security.Claims;

namespace Emm.Application.Abstractions;

public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string AccessTokenJti { get; set; } = string.Empty;
    public string RefreshTokenJti { get; set; } = string.Empty;
    public long AccessTokenExpiresAt { get; set; }
    public long RefreshTokenExpiresAt { get; set; }
}

public interface IJwtService
{
    TokenResult GenerateTokens(object userId, string email);
    TokenResult GenerateTokens(object userId, string email, Action<ClaimsIdentity> action);
    bool ValidateAccessToken(string token, out string userId);
    bool ValidateRefreshToken(string token, out string userId);
    bool ValidateAccessToken(string token, out string userId, out string jti);
    bool ValidateRefreshToken(string token, out string userId, out string jti);
    TokenResult RefreshTokens(string refreshToken);
}
