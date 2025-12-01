using Emm.Domain.Entities;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;


public class UserRepository : GenericRepository<User, long>, IUserRepository
{
    public UserRepository(XDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken: cancellationToken);
    }
}