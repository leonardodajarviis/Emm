using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;
using Emm.Domain.Entities.Operations;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddShiftLogCommandHandler : IRequestHandler<AddShiftLogCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _shiftRepository;
    private readonly IShiftLogRepository _shiftLogRepository;
    private readonly IQueryContext _qq;

    public AddShiftLogCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository shiftRepository,
        IShiftLogRepository taskRepository,
        IQueryContext queryContext
    )
    {
        _unitOfWork = unitOfWork;
        _shiftRepository = shiftRepository;
        _shiftLogRepository = taskRepository;
        _qq = queryContext;
    }

    public async Task<Result<object>> Handle(AddShiftLogCommand request, CancellationToken cancellationToken)
    {
        // Verify shift exists
        var shift = await _shiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        if (shift.Assets.Count == 0)
        {
            return Result<object>.Validation(
                "Operation shift has no associated assets",
                "SHIFT_NO_ASSETS");
        }

        var assetIds = shift.Assets.Select(a => a.AssetId).ToArray();

        var assetParameterDict = _qq.Query<AssetParameter>()
            .Where(ap => assetIds.Contains(ap.AssetId))
            .ToDictionary(ap => (ap.AssetId, ap.ParameterId), ap => ap);

        var assetDict = shift.Assets
            .Distinct()
            .ToDictionary(a => a.AssetId, a => a);

        // Create new task aggregate
        var newShiftLog = new ShiftLog(
            request.OperationShiftId,
            request.Name,
            "noDescription",
            request.StartTime,
            request.EndTime);

        // Add readings if provided
        if (request.Readings != null && request.Readings.Any())
        {
            foreach (var reading in request.Readings)
            {
                var asset = assetDict.GetValueOrDefault(reading.AssetId);
                if (asset == null)
                {
                    return Result<object>.Failure(ErrorType.Validation, $"Asset with ID {reading.AssetId} is not associated with the operation shift");
                }

                var parameter = assetParameterDict.GetValueOrDefault((reading.AssetId, reading.ParameterId));
                if (parameter == null)
                {
                    return Result<object>.Failure(ErrorType.Validation, $"Parameter with ID {reading.ParameterId} is not associated with asset ID {reading.AssetId}");
                }

                newShiftLog.AddReading(
                    assetId: reading.AssetId,
                    assetCode: asset.AssetCode,
                    assetName: asset.AssetName,
                    parameterId: reading.ParameterId,
                    parameterName: parameter.ParameterName ?? "unknown",
                    parameterCode: parameter.ParameterCode ?? "unknown",
                    value: reading.Value,
                    unit: parameter.ParameterUnit ?? "unknown",
                    shiftLogCheckPointLinkedId: reading.TaskCheckpointLinkedId);
            }
        }

        // Add checkpoints if provided
        if (request.Checkpoints != null && request.Checkpoints.Any())
        {
            foreach (var checkpoint in request.Checkpoints)
            {
                newShiftLog.AddCheckpoint(
                    linkedId: checkpoint.LinkedId,
                    name: checkpoint.Name,
                    locationId: checkpoint.LocationId,
                    locationName: "",
                    isWithAttachedMaterial: checkpoint.IsWithAttachedMaterial,
                    itemId: checkpoint.ItemId);
            }
        }

        // Add status history events if provided
        if (request.Events != null && request.Events.Any())
        {
            foreach (var history in request.Events)
            {
                newShiftLog.RecordEvent(
                    eventType: history.EventType,
                    startTime: history.StartTime);
            }
        }

        var itemIds = request.Items?.Select(i => i.ItemId).Distinct().ToArray() ?? [];
        var itemDict = _qq.Query<Item>()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionary(i => i.Id, i => i);

        // Add items if provided
        if (request.Items != null && request.Items.Any())
        {
            foreach (var item in request.Items)
            {
                var asset = assetDict.GetValueOrDefault(item.AssetId);
                if (asset == null)
                {
                    return Result<object>.Failure(ErrorType.Validation, $"Asset with ID {item.AssetId} is not associated with the operation shift");
                }
                var itemInfo = itemDict.GetValueOrDefault(item.ItemId);
                if (itemInfo == null)
                {
                    return Result<object>.Failure(ErrorType.Validation, $"Item with ID {item.ItemId} does not exist");
                }

                newShiftLog.AddItem(
                    itemId: item.ItemId,
                    itemName: itemInfo.Name,
                    quantity: item.Quantity,
                    assetId: item.AssetId,
                    assetCode: asset.AssetCode,
                    assetName: asset.AssetName,
                    unitOfMeasureId: itemInfo.UnitOfMeasureId);
            }
        }

        // Add task to repository
        await _shiftLogRepository.AddAsync(newShiftLog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Task added successfully");
    }
}
