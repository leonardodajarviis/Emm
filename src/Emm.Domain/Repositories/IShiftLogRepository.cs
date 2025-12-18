using Emm.Domain.Entities.Operations;

namespace Emm.Domain.Repositories;

public interface IShiftLogRepository : IRepository<ShiftLog, Guid>
{
    Task<IEnumerable<ShiftLog>> GetByShiftIdAsync(Guid shiftId, CancellationToken cancellationToken = default);
    Task<ShiftLog?> GetTaskWithDetailsAsync(Guid taskId, CancellationToken cancellationToken = default);
}
