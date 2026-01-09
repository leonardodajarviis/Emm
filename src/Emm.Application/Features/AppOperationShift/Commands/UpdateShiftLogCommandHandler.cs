using Emm.Application.ErrorCodes;
using Emm.Application.Features.AppOperationShift.Builder;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class UpdateShiftLogCommandHandler : IRequestHandler<UpdateShiftLogCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftLogRepository _shiftLogRepository;
    private readonly IOperationShiftRepository _shiftRepository;
    private readonly IEnumerable<IUpdateShiftLogBuilderHandler> _handlers;

    public UpdateShiftLogCommandHandler(
        IUnitOfWork unitOfWork,
        IShiftLogRepository shiftLogRepository,
        IOperationShiftRepository shiftRepository,
        IEnumerable<IUpdateShiftLogBuilderHandler> handlers)
    {
        _unitOfWork = unitOfWork;
        _shiftLogRepository = shiftLogRepository;
        _shiftRepository = shiftRepository;
        _handlers = handlers;
    }

    public async Task<Result> Handle(UpdateShiftLogCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        // Load and validate entities
        var shift = await _shiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift == null)
        {
            return Result.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var shiftLog = await _shiftLogRepository.GetByIdAsync(request.ShiftLogId, cancellationToken);
        if (shiftLog == null)
        {
            return Result.NotFound("Shift log not found", ShiftLogErrorCodes.NotFound);
        }

        // Verify task belongs to shift
        if (shiftLog.OperationShiftId != request.OperationShiftId)
        {
            return Result.Conflict("Shift log does not belong to this shift", "SHIFT_LOG_MISMATCH");
        }

        var updateContext = new UpdateShiftLogContext
        {
            ShiftLog = shiftLog,
            AssetDict = shift.Assets.ToDictionary(a => a.AssetId, a => a),
            Data = data
        };

        var result = await ExecuteChainAsync(updateContext, cancellationToken);
        if (!result.IsSuccess)
        {
            return result;
        }
        shiftLog.RaiseReadingEvents();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<Result> ExecuteChainAsync(
        UpdateShiftLogContext context,
        CancellationToken cancellationToken)
    {
        var handlers = _handlers.ToList();
        async Task<Result> Next(int index)
        {
            if (index < handlers.Count)
            {
                var handler = handlers[index];
                var result = await handler.Handle(context, cancellationToken);
                if (!result.IsSuccess)
                {
                    return result;
                }
                return await Next(index + 1);
            }
            return Result.Success();
        }

        return await Next(0);
    }
}
