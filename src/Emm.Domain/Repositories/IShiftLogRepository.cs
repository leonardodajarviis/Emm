using Emm.Domain.Entities.Operations;

namespace Emm.Domain.Repositories;

public interface IShiftLogRepository : IRepository<ShiftLog, long>
{
    Task<IEnumerable<ShiftLog>> GetByShiftIdAsync(long shiftId, CancellationToken cancellationToken = default);
    Task<ShiftLog?> GetTaskWithDetailsAsync(long taskId, CancellationToken cancellationToken = default);
}
