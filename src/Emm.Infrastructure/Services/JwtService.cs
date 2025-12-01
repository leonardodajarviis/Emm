using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Emm.Application.Abstractions;
using Emm.Infrastructure.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Emm.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<JwtService> _logger;

    public JwtService(IOptions<JwtOptions> jwtOptions, ILogger<JwtService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public TokenResult GenerateTokens(object userId, string email)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var userIdStr = userId.ToString() ?? throw new ArgumentNullException(nameof(userId));

        var (accessToken, accessTokenExpiresAt, accessJti) = GenerateToken(userIdStr, email, _jwtOptions.AccessKey, _jwtOptions.AccessExpiresIn);
        var (refreshToken, refreshTokenExpiresAt, refreshJti) = GenerateToken(userIdStr, email, _jwtOptions.RefreshKey, _jwtOptions.RefreshExpiresIn);

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenJti = accessJti,
            RefreshTokenJti = refreshJti,
            AccessTokenExpiresAt = ToUnixTimeSeconds(accessTokenExpiresAt),
            RefreshTokenExpiresAt = ToUnixTimeSeconds(refreshTokenExpiresAt)
        };
    }

    public TokenResult GenerateTokens(object userId, string email, Action<ClaimsIdentity> action)
    {
        ArgumentNullException.ThrowIfNull(userId);
        var userIdStr = userId.ToString() ?? throw new ArgumentNullException(nameof(userId));

        var (accessToken, accessTokenExpiresAt, accessJti) = GenerateToken(userIdStr, email, _jwtOptions.AccessKey, _jwtOptions.AccessExpiresIn, action);
        var (refreshToken, refreshTokenExpiresAt, refreshJti) = GenerateToken(userIdStr, email, _jwtOptions.RefreshKey, _jwtOptions.RefreshExpiresIn, action);
        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenJti = accessJti,
            RefreshTokenJti = refreshJti,
            AccessTokenExpiresAt = ToUnixTimeSeconds(accessTokenExpiresAt),
            RefreshTokenExpiresAt = ToUnixTimeSeconds(refreshTokenExpiresAt)
        };
    }

    private static (string token, DateTime expiresAt, string jti) GenerateToken(string userId, string email, string keyStr, int expiresIn, Action<ClaimsIdentity>? action = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(keyStr);
        var expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
        var jti = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            ]),
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        action?.Invoke(tokenDescriptor.Subject);

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiresAt, jti);
    }

    private static long ToUnixTimeSeconds(DateTime dateTime)
    {
        return (long)(dateTime.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public bool ValidateAccessToken(string token, out string userId)
    {
        return ValidateToken(token, _jwtOptions.AccessKey, out userId);
    }

    public bool ValidateRefreshToken(string token, out string userId)
    {
        return ValidateToken(token, _jwtOptions.RefreshKey, out userId);
    }

    public bool ValidateAccessToken(string token, out string userId, out string jti)
    {
        return ValidateToken(token, _jwtOptions.AccessKey, out userId, out jti);
    }

    public bool ValidateRefreshToken(string token, out string userId, out string jti)
    {
        return ValidateToken(token, _jwtOptions.RefreshKey, out userId, out jti);
    }

    private bool ValidateToken(string token, string keyStr, out string userId)
    {
        return ValidateToken(token, keyStr, out userId, out _);
    }

    private bool ValidateToken(string token, string keyStr, out string userId, out string jti)
    {
        userId = Guid.Empty.ToString();
        jti = string.Empty;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(keyStr);
        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.FromSeconds(30) // Allow 30 seconds clock skew tolerance
            }, out _);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (userIdClaim != null)
            {
                userId = userIdClaim;
                jti = jtiClaim ?? string.Empty;
                return true;
            }

            _logger.LogWarning("Token validation failed: UserId claim not found in token");
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: Token has expired");
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: Invalid token signature");
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token validation");
        }
        return false;
    }

    public TokenResult RefreshTokens(string refreshToken)
    {
        if (ValidateRefreshToken(refreshToken, out var userId))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(refreshToken);
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;

            // Preserve all custom claims from the original token
            return GenerateTokens(userId, email, identity =>
            {
                // Copy all claims except standard ones that will be regenerated
                var claimsToPreserve = jwtToken.Claims
                    .Where(c => c.Type != ClaimTypes.NameIdentifier &&
                               c.Type != ClaimTypes.Email &&
                               c.Type != JwtRegisteredClaimNames.Jti &&
                               c.Type != JwtRegisteredClaimNames.Exp &&
                               c.Type != JwtRegisteredClaimNames.Nbf &&
                               c.Type != JwtRegisteredClaimNames.Iat)
                    .ToList();

                foreach (var claim in claimsToPreserve)
                {
                    identity.AddClaim(new Claim(claim.Type, claim.Value));
                }
            });
        }
        throw new SecurityTokenException("Invalid refresh token");
    }
}
