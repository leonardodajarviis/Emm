using Emm.Domain.Entities;

namespace Emm.Domain.Repositories;

public interface IUserSessionRepository : IRepository<UserSession, Guid>
{
    Task<UserSession?> GetByRefreshTokenJtiAsync(string jti, CancellationToken cancellationToken = default);
    Task<UserSession?> GetByAccessTokenJtiAsync(string jti, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
