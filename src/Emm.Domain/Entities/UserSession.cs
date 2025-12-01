using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities;

public class UserSession : AggregateRoot
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string AccessTokenJti { get; private set; } = null!;
    public string RefreshTokenJti { get; private set; } = null!;
    public DateTime AccessTokenExpiresAt { get; private set; }
    public DateTime RefreshTokenExpiresAt { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt.HasValue;

    // Navigation property
    public User User { get; private set; } = null!;

    private UserSession() { } // For EF Core

    public UserSession(
        long userId,
        string accessTokenJti,
        string refreshTokenJti,
        DateTime accessTokenExpiresAt,
        DateTime refreshTokenExpiresAt,
        string? ipAddress = null,
        string? userAgent = null)
    {
        UserId = userId;
        AccessTokenJti = accessTokenJti;
        RefreshTokenJti = refreshTokenJti;
        AccessTokenExpiresAt = accessTokenExpiresAt;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
    }

    public void UpdateTokens(
        string accessTokenJti,
        string refreshTokenJti,
        DateTime accessTokenExpiresAt,
        DateTime refreshTokenExpiresAt)
    {
        AccessTokenJti = accessTokenJti;
        RefreshTokenJti = refreshTokenJti;
        AccessTokenExpiresAt = accessTokenExpiresAt;
        RefreshTokenExpiresAt = refreshTokenExpiresAt;
    }
}
