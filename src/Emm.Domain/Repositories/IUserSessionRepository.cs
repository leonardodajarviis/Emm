using Emm.Domain.Entities;

namespace Emm.Domain.Repositories;

public interface IUserSessionRepository : IRepository<UserSession, long>
{
    Task<UserSession?> GetByRefreshTokenJtiAsync(string jti, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByAccessTokenJtiAsync(string jti, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(long userId, CancellationToken cancellationToken = default);
}
