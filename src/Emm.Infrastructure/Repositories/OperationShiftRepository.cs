using Emm.Domain.Entities.Operations;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class OperationShiftRepository : GenericRepository<OperationShift, long>, IOperationShiftRepository
{
    public OperationShiftRepository(XDbContext context) : base(context)
    {
    }

    public override async Task<OperationShift?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Assets)
            .FirstOrDefaultAsync(shift => shift.Id == id, cancellationToken: cancellationToken);
    }
}
