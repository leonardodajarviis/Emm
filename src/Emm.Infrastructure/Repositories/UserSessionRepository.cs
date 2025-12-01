using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class UserSessionRepository : GenericRepository<UserSession, long>, IUserSessionRepository
{
    public UserSessionRepository(XDbContext context) : base(context)
    {
    }

    public async Task<UserSession?> GetByRefreshTokenJtiAsync(string jti, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.RefreshTokenJti == jti && s.RevokedAt == null, cancellationToken);
    }

    public async Task<UserSession?> GetByAccessTokenJtiAsync(string jti, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.AccessTokenJti == jti && s.RevokedAt == null, cancellationToken);
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.UserId == userId && s.RevokedAt == null && s.RefreshTokenExpiresAt > DateTime.UtcNow)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeAllUserSessionsAsync(long userId, CancellationToken cancellationToken = default)
    {
        var sessions = await DbSet
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var session in sessions)
        {
            session.Revoke();
        }
    }
}
