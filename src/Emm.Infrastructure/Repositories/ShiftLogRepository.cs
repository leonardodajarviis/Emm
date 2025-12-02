using Emm.Domain.Entities.Operations;
using Emm.Domain.Repositories;
using Emm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Repositories;

public class ShiftLogRepository : GenericRepository<ShiftLog, long>, IShiftLogRepository
{
    public ShiftLogRepository(XDbContext context) : base(context)
    {
    }

    public override async Task<Domain.Entities.Operations.ShiftLog?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Readings)
            .Include(t => t.Checkpoints)
            .Include(t => t.Events)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<ShiftLog>> GetByShiftIdAsync(long shiftId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(t => t.Readings)
            .Include(t => t.Checkpoints)
            .Include(t => t.Events)
            .Where(t => t.OperationShiftId == shiftId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShiftLog?> GetTaskWithDetailsAsync(long taskId, CancellationToken cancellationToken = default)
    {
        return await GetByIdAsync(taskId, cancellationToken);
    }
}
