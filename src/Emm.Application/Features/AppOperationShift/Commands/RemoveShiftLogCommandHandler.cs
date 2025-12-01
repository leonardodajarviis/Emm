using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class RemoveShiftLogCommandHandler : IRequestHandler<RemoveShiftLogCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, long> _shiftRepository;
    private readonly IShiftLogRepository _shiftLogRepository;

    public RemoveShiftLogCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<OperationShift, long> shiftRepository,
        IShiftLogRepository taskRepository)
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
        _shiftLogRepository = taskRepository;
    }

    public async Task<Result<object>> Handle(RemoveShiftLogCommand request, CancellationToken cancellationToken)
    {
        // Verify shift exists and check status
        var shift = await _shiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        // Business rule: Cannot remove tasks from shift in InProgress or Completed status
        if (shift.Status == OperationShiftStatus.InProgress || shift.Status == OperationShiftStatus.Completed)
        {
            return Result<object>.Conflict(
                $"Cannot remove tasks from shift in {shift.Status} status",
                "SHIFT_LOG_CANNOT_REMOVE_IN_PROGRESS");
        }

        // Get task to remove
        var task = await _shiftLogRepository.GetByIdAsync(request.ShiftLogId, cancellationToken);
        if (task == null)
        {
            return Result<object>.NotFound("Shift log not found", ShiftLogErrorCodes.NotFound);
        }

        // Verify task belongs to shift
        if (task.OperationShiftId != request.OperationShiftId)
        {
            return Result<object>.Conflict("Shift log does not belong to this shift", "SHIFT_LOG_MISMATCH");
        }

        // Remove task
        _shiftLogRepository.Remove(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Shift Log removed successfully");
    }
}
