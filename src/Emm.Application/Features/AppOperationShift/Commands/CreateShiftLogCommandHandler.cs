using Emm.Application.ErrorCodes;
using Emm.Application.Features.AppOperationShift.Builder;
using Emm.Domain.Entities.AssetCatalog;
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

        if (shift == null)
        {
            return Result.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var assetIds = shift.Assets.Select(a => a.AssetId).ToArray();

        var assetParameterDict = await _queryContext.Query<AssetParameter>()
            .Where(ap => assetIds.Contains(ap.AssetId))
            .ToDictionaryAsync(ap => (ap.AssetId, ap.ParameterId), ap => ap);

        var logCount = await _queryContext.Query<ShiftLog>()
            .CountAsync(sl => sl.OperationShiftId == request.OperationShiftId, cancellationToken);

        var prevShiftLog = await _queryContext.Query<ShiftLog>()
            .Include(sl => sl.Readings)
            .Where(sl => sl.OperationShiftId == shift.CurrentShiftLogId)
            .FirstOrDefaultAsync(cancellationToken);

        var logOrder = logCount + 1;

        // Create new task aggregate
        var shiftLog = new ShiftLog(
            logOrder,
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
            AssetDict = shift.Assets.ToDictionary(a => a.AssetId, a => a),
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
