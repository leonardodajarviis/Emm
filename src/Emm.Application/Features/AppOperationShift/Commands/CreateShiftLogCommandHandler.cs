using Emm.Application.ErrorCodes;
using Emm.Application.Features.AppOperationShift.Builder;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CreateShiftLogCommandHandler : IRequestHandler<CreateShiftLogCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _shiftRepository;
    private readonly IShiftLogRepository _shiftLogRepository;
    private readonly IQueryContext _queryContext;
    private readonly IEnumerable<ICreateShiftLogBuilderHandler> _handlers;

    public CreateShiftLogCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository shiftRepository,
        IShiftLogRepository taskRepository,
        IEnumerable<ICreateShiftLogBuilderHandler> handlers,
        IQueryContext queryContext
    )
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
        _shiftLogRepository = taskRepository;
        _queryContext = queryContext;
        _handlers = handlers;
    }

    public async Task<Result> Handle(CreateShiftLogCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        // Verify shift exists
        var shift = await _shiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        var assetsVerified = new List<OperationShiftAsset>();

        if (shift == null)
        {
            return Result.NotFound("Không tìm thấy ca vận hành");
        }

        var logBatch = string.Empty;
        if (data.AssetId.HasValue)
        {
            var asset = shift.Assets.FirstOrDefault(a => a.AssetId == data.AssetId.Value);
            if (asset == null)
            {
                return Result.Validation("Tài sản không thuộc ca vận hành này", ShiftLogErrorCodes.AssetNotInOperationShift);
            }
            assetsVerified.Add(asset);
            logBatch = $"{shift.Code}|ASSET:{asset.AssetId}";
        }

        if (data.BoxId.HasValue)
        {
            var box = shift.AssetBoxes.FirstOrDefault(a => a.Id == data.BoxId.Value);
            if (box == null)
            {
                return Result.Validation("Nhóm thiết bị cùng vận hành không thuộc ca vận hành này", ShiftLogErrorCodes.AssetBoxNotInOperationShift);
            }
            assetsVerified.AddRange(shift.Assets.Where(a => a.AssetBoxId == box.Id));
            logBatch = $"{shift.Code}|Box:{box.Id}";
        }

        var lastLog = await _queryContext.Query<ShiftLog>()
            .OrderByDescending(sl => sl.LogOrder)
            .FirstOrDefaultAsync(sl => sl.OperationShiftId == request.OperationShiftId && sl.Batch == logBatch, cancellationToken);

        if (lastLog != null && data.StartTime < lastLog?.EndTime)
        {
            return Result.Validation("Thời gian bắt đầu nhật ký ca vận hành không được trước thời gian kết thúc của nhật ký trước đó", ShiftLogErrorCodes.InvalidTime);
        }

        var logOrder = lastLog?.LogOrder ?? 0 + 1;

        // Create new task aggregate
        var shiftLog = new ShiftLog(
            logOrder,
            logBatch,
            request.OperationShiftId,
            data.Name,
            data.StartTime,
            data.EndTime,
            data.AssetId,
            data.BoxId,
            data.LocationId,
            data.LocationName);

        shift.SetCurrentShiftLog(shiftLog.Id);

        var shiftLogCtx = new CreateShiftLogContext
        {
            ShiftLog = shiftLog,
            AssetDict = assetsVerified.ToDictionary(a => a.AssetId, a => a),
            Data = data
        };

        var result = await ExecuteChainAsync(shiftLogCtx, cancellationToken);
        if (!result.IsSuccess)
        {
            return result;
        }

        // Add task to repository
        shiftLog.RaiseReadingEvents();
        await _shiftLogRepository.AddAsync(shiftLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new
        {
            shiftLog.Id,
        });
    }

    private async Task<Result> ExecuteChainAsync(
        CreateShiftLogContext context,
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
