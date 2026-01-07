using Emm.Application.ErrorCodes;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Inventory;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class UpdateShiftLogCommandHandler : IRequestHandler<UpdateShiftLogCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IShiftLogRepository _taskRepository;
    private readonly IOperationShiftRepository _shiftRepository;
    private readonly IQueryContext _qq;

    public UpdateShiftLogCommandHandler(
        IUnitOfWork unitOfWork,
        IShiftLogRepository taskRepository,
        IOperationShiftRepository shiftRepository,
        IQueryContext queryContext)
    {
        _unitOfWork = unitOfWork;
        _taskRepository = taskRepository;
        _shiftRepository = shiftRepository;
        _qq = queryContext;
    }

    public async Task<Result> Handle(UpdateShiftLogCommand request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);

        if (shift == null)
        {
            return Result.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var assetIds = shift.Assets.Select(a => a.AssetId).ToArray();

        var assetParameterDict = _qq.Query<AssetParameter>()
            .Where(ap => assetIds.Contains(ap.AssetId))
            .ToDictionary(ap => (ap.AssetId, ap.ParameterId), ap => ap);

        var assetDict = shift.Assets
            .Distinct()
            .ToDictionary(a => a.AssetId, a => a);

        var shiftLog = await _taskRepository.GetByIdAsync(request.ShiftLogId, cancellationToken);
        if (shiftLog == null)
        {
            return Result.NotFound("Shift log not found", ShiftLogErrorCodes.NotFound);
        }

        shiftLog.UpdateStartTime(request.StartTime);
        if (request.EndTime.HasValue)
        {
            shiftLog.UpdateEndTime(request.EndTime.Value);
        }

        // Verify task belongs to shift
        if (shiftLog.OperationShiftId != request.OperationShiftId)
        {
            return Result.Conflict("Shift log does not belong to this shift", "SHIFT_LOG_MISMATCH");
        }

        // Smart update for readings: add new, update existing, remove missing
        if (request.Readings != null)
        {
            var requestReadingIds = request.Readings
                .Where(r => r.Id.HasValue)
                .Select(r => r.Id!.Value)
                .ToHashSet();

            // Remove readings not in request
            var readingsToRemove = shiftLog.Readings
                .Where(r => !requestReadingIds.Contains(r.Id))
                .Select(r => r.Id)
                .ToList();

            foreach (var readingId in readingsToRemove)
            {
                shiftLog.RemoveReading(readingId);
            }

            // Add new or update existing readings
            foreach (var reading in request.Readings)
            {
                if (reading.Id.HasValue)
                {
                    // Update existing
                    var existingReading = shiftLog.GetReading(reading.Id.Value);
                    existingReading.UpdateValue(reading.Value);
                }
                else
                {
                    var asset = assetDict.GetValueOrDefault(reading.AssetId);
                    if (asset == null)
                    {
                        return Result.Failure(ErrorType.Validation, $"Asset with ID {reading.AssetId} is not associated with the operation shift");
                    }
                    var parameter = assetParameterDict.GetValueOrDefault((reading.AssetId, reading.ParameterId));
                    if (parameter == null)
                    {
                        return Result.Failure(ErrorType.Validation, $"Parameter with ID {reading.ParameterId} is not associated with asset ID {reading.AssetId}");
                    }
                    // Add new
                    shiftLog.AddReading(
                        assetId: reading.AssetId,
                        assetCode: asset.AssetCode,
                        assetName: asset.AssetName,
                        parameterId: reading.ParameterId,
                        parameterName: parameter.ParameterName,
                        parameterCode: parameter.ParameterCode,
                        value: reading.Value,
                        unitOfMeasureId: parameter.UnitOfMeasureId,
                        shiftLogCheckPointLinkedId: reading.TaskCheckpointLinkedId);
                }
            }
        }

        // Smart update for checkpoints: add new, update existing, remove missing
        if (request.Checkpoints != null)
        {
            var requestCheckpointIds = request.Checkpoints
                .Where(c => c.Id.HasValue)
                .Select(c => c.Id!.Value)
                .ToHashSet();

            // Remove checkpoints not in request
            var checkpointsToRemove = shiftLog.Checkpoints
                .Where(c => !requestCheckpointIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToList();

            foreach (var checkpointId in checkpointsToRemove)
            {
                shiftLog.RemoveCheckpoint(checkpointId);
            }

            // Add new or update existing checkpoints
            foreach (var checkpoint in request.Checkpoints)
            {
                if (checkpoint.Id.HasValue)
                {
                    // Update existing
                    var existingCheckpoint = shiftLog.GetCheckpoint(checkpoint.Id.Value);
                    existingCheckpoint.Update(
                        name: checkpoint.Name,
                        locationId: checkpoint.LocationId,
                        locationName: "",
                        isWithAttachedMaterial: checkpoint.IsWithAttachedMaterial,
                        itemId: checkpoint.ItemId);
                }
                else
                {
                    // Add new
                    shiftLog.AddCheckpoint(
                        linkedId: checkpoint.LinkedId,
                        name: checkpoint.Name,
                        locationId: checkpoint.LocationId,
                        locationName: "",
                        isWithAttachedMaterial: checkpoint.IsWithAttachedMaterial,
                        itemId: checkpoint.ItemId);
                }
            }
        }

        // Smart update for status history: add new, update existing, remove missing
        if (request.Events != null)
        {
            var requestEventIds = request.Events
                .Where(h => h.Id.HasValue)
                .Select(h => h.Id!.Value)
                .ToHashSet();

            // Remove events not in request
            var eventsToRemove = shiftLog.Events
                .Where(h => !requestEventIds.Contains(h.Id))
                .Select(h => h.Id)
                .ToList();

            foreach (var eventId in eventsToRemove)
            {
                shiftLog.RemoveEvent(eventId);
            }

            // Add new or update existing events
            foreach (var history in request.Events)
            {
                if (history.Id.HasValue)
                {
                    // Update existing
                    shiftLog.UpdateEvent(history.Id.Value, history.EventType, history.StartTime, history.EndTime);
                }
                else
                {
                    // Add new
                    shiftLog.RecordEvent(
                        eventType: history.EventType,
                        startTime: history.StartTime,
                        endTime: history.EndTime);
                }
            }
        }

        // Smart update for items: add new, update existing, remove missing
        var itemIds = request.Items?.Select(i => i.ItemId).Distinct().ToArray() ?? [];
        var itemDict = _qq.Query<Item>()
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionary(i => i.Id, i => i);

        if (request.Items != null)
        {
            var requestItemIds = request.Items
                .Where(i => i.Id.HasValue)
                .Select(i => i.Id!.Value)
                .ToHashSet();

            // Remove items not in request
            var itemsToRemove = shiftLog.Items
                .Where(i => !requestItemIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToList();

            foreach (var itemId in itemsToRemove)
            {
                shiftLog.RemoveItem(itemId);
            }

            // Add new items (items without Id)
            foreach (var item in request.Items)
            {
                if (!item.Id.HasValue)
                {
                    var asset = assetDict.GetValueOrDefault(item.AssetId);
                    if (asset == null)
                    {
                        return Result.Failure(ErrorType.Validation, $"Asset with ID {item.AssetId} is not associated with the operation shift");
                    }
                    var itemInfo = itemDict.GetValueOrDefault(item.ItemId);
                    if (itemInfo == null)
                    {
                        return Result.Failure(ErrorType.Validation, $"Item with ID {item.ItemId} does not exist");
                    }

                    shiftLog.AddItem(
                        itemId: item.ItemId,
                        itemName: itemInfo.Name,
                        quantity: item.Quantity,
                        assetId: item.AssetId,
                        assetCode: asset.AssetCode,
                        assetName: asset.AssetName,
                        unitOfMeasureId: itemInfo.UnitOfMeasureId);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
