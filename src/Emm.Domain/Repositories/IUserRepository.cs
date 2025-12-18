using Emm.Domain.Entities;

namespace Emm.Domain.Repositories;


public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}