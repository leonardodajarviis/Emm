using Emm.Application.ErrorCodes;
using Emm.Application.Features.AppOperationShift.Builder;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class UpdateShiftLogCommandHandler : IRequestHandler<UpdateShiftLogCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftLogRepository _shiftLogRepository;
    private readonly IOperationShiftRepository _shiftRepository;
    private readonly IEnumerable<IUpdateShiftLogBuilderHandler> _handlers;
    private readonly IQueryContext _queryContext;

    public UpdateShiftLogCommandHandler(
        IUnitOfWork unitOfWork,
        IShiftLogRepository shiftLogRepository,
        IOperationShiftRepository shiftRepository,
        IQueryContext queryContext,
        IEnumerable<IUpdateShiftLogBuilderHandler> handlers)
    {
        _unitOfWork = unitOfWork;
        _shiftLogRepository = shiftLogRepository;
        _shiftRepository = shiftRepository;
        _handlers = handlers;
        _queryContext = queryContext;
    }

    public async Task<Result> Handle(UpdateShiftLogCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var shift = await _shiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift == null)
        {
            return Result.NotFound("Không tìm thấy ca vận hành");
        }

        var shiftLog = await _shiftLogRepository.GetByIdAsync(request.ShiftLogId, cancellationToken);
        if (shiftLog == null)
        {
            return Result.NotFound("Không tìm thấy nhật ký ca vận hành");
        }

        if (shiftLog.OperationShiftId != request.OperationShiftId)
        {
            return Result.Conflict("Nhật ký ca vận hành không thuộc ca vận hành này", ShiftLogErrorCodes.ShiftLogMissmatch);
        }

        var prevTime = await _queryContext.Query<ShiftLog>()
            .Where(sl => sl.OperationShiftId == request.OperationShiftId && sl.Id != shiftLog.Id)
            .Where(sl => sl.LogOrder == shiftLog.LogOrder - 1)
            .Select(sl => sl.EndTime)
            .FirstOrDefaultAsync(cancellationToken);

        if (prevTime != null && data.StartTime < prevTime)
        {
            return Result.Validation("Thời gian bắt đầu nhật ký ca vận hành không được trước thời gian kết thúc của nhật ký trước đó", ShiftLogErrorCodes.InvalidTime);
        }

        var nextTime = await _queryContext.Query<ShiftLog>()
            .Where(sl => sl.OperationShiftId == request.OperationShiftId && sl.Id != shiftLog.Id)
            .Where(sl => sl.LogOrder == shiftLog.LogOrder + 1)
            .Select(sl => new { sl.StartTime })
            .FirstOrDefaultAsync(cancellationToken);

        if (nextTime != null && data.EndTime > nextTime.StartTime)
        {
            return Result.Validation("Thời gian kết thúc nhật ký ca vận hành không được sau thời gian bắt đầu của nhật ký sau đó", ShiftLogErrorCodes.InvalidTime);
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
