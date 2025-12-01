using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Data;

public sealed class QueryContext : IQueryContext
{
    private readonly XDbContext _context;
    public QueryContext(XDbContext context)
    {
        _context = context;
    }

    public IQueryable<T> Query<T>() where T : class
    {
        return _context.Set<T>().AsNoTracking();
    }

}
