using Emm.Domain.Entities.Operations;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class OperationShiftRepository : GenericRepository<OperationShift, Guid>, IOperationShiftRepository
{
    public OperationShiftRepository(XDbContext context) : base(context)
    {
    }

    public override async Task<OperationShift?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Assets)
            .Include(s => s.AssetBoxes)
            .FirstOrDefaultAsync(shift => shift.Id == id, cancellationToken: cancellationToken);
    }
}
