using Emm.Domain.Entities;

namespace Emm.Domain.Repositories;


public interface IUserRepository : IRepository<User, long>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}