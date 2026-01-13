using Emm.Application.Helpers;
using Emm.Application.Services;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class SyncShiftLogReadingsHandler : IUpdateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;
    private readonly IReadingValueValidator _readingValueValidator;
    private readonly IDateTimeProvider _clock;

    public SyncShiftLogReadingsHandler(
        IQueryContext queryContext,
        IReadingValueValidator readingValueValidator,
        IDateTimeProvider clock)
    {
        _queryContext = queryContext;
        _readingValueValidator = readingValueValidator;
        _clock = clock;
    }
    public async Task<Result> Handle(UpdateShiftLogContext context, CancellationToken cancellationToken)
    {
        var readings = context.Data.Readings;
        var shiftLog = context.ShiftLog;
        var requestReadingIds = readings
            .Where(r => r.Id.HasValue)
            .Select(r => r.Id!.Value)
            .ToHashSet();

        CollectionHelper.RemoveItemsNotInRequest(
            context.ShiftLog.Readings,
            requestReadingIds,
            r => r.Id,
            shiftLog.RemoveItem);

        var assetIds = readings.Select(r => r.AssetId).Distinct().ToArray();

        var assetParameterDict = await _queryContext.Query<AssetParameter>()
            .Where(ap => assetIds.Contains(ap.AssetId))
            .ToDictionaryAsync(ap => (ap.AssetId, ap.ParameterId), ap => ap, cancellationToken);

        var allReading = await _queryContext.Query<ShiftLogParameterReading>()
            .Where(r => r.OperationShiftId == shiftLog.OperationShiftId)
            .ToListAsync(cancellationToken);

        // Lấy reading cuối cùng của từng parameter theo groupNumber
        var lastReadingsByParameter = allReading
            .GroupBy(r => new { r.AssetId, r.ParameterId })
            .ToDictionary(
                g => (g.Key.AssetId, g.Key.ParameterId),
                g => g.OrderByDescending(r => r.GroupNumber).FirstOrDefault());

        var nextNumber = allReading.Count > 0 ? allReading.Max(r => r.GroupNumber) + 1 : 1;

        // Chuẩn bị dictionary cho validation
        var previousValuesByParameter = new Dictionary<(Guid AssetId, Guid ParameterId), decimal>();
        if (nextNumber > 1)
        {
            // Lấy giá trị cuối cùng của các parameter snapshot
            previousValuesByParameter = lastReadingsByParameter
                .Where(x => x.Value != null && x.Value.ParameterType == ParameterType.Snapshot)
                .ToDictionary(x => x.Key, x => x.Value!.Value);
        }
        else
        {
            // Nếu là batch đầu tiên, lấy từ snapshot
            previousValuesByParameter = await _queryContext.Query<OperationShiftReadingSnapshot>()
                .Where(r => r.OperationShiftId == shiftLog.OperationShiftId)
                .GroupBy(r => new { r.AssetId, r.ParameterId })
                .Select(g => new
                {
                    g.Key.AssetId,
                    g.Key.ParameterId,
                    LastSnapshotValue = g.FirstOrDefault()!.Value
                })
                .ToDictionaryAsync(
                    x => (x.AssetId, x.ParameterId),
                    x => x.LastSnapshotValue,
                    cancellationToken);
        }

        // Dictionary để tracking các readings được update
        var updateReadingDict = allReading.ToDictionary(r => r.Id);

        // Tách readings thành 2 nhóm: update và add new
        var readingsToUpdate = readings.Where(r => r.Id.HasValue).ToList();
        var readingsToAdd = readings.Where(r => !r.Id.HasValue).ToList();

        // Validate readings cần update
        if (readingsToUpdate.Count > 0)
        {
            var updateValidationResult = _readingValueValidator.ValidateBatchMonotonicProgression(
                readingsToUpdate,
                previousValuesByParameter,
                r =>
                {
                    // Lấy reading hiện tại để xác định assetId
                    var existingReading = updateReadingDict.GetValueOrDefault(r.Id!.Value);
                    return existingReading?.AssetId ?? Guid.Empty;
                },
                r =>
                {
                    // Lấy reading hiện tại để xác định parameterId
                    var existingReading = updateReadingDict.GetValueOrDefault(r.Id!.Value);
                    return existingReading?.ParameterId ?? Guid.Empty;
                },
                r => r.Value,
                r =>
                {
                    var existingReading = updateReadingDict.GetValueOrDefault(r.Id!.Value);
                    return existingReading?.AssetCode ?? r.AssetId.ToString();
                },
                r =>
                {
                    var existingReading = updateReadingDict.GetValueOrDefault(r.Id!.Value);
                    return existingReading?.ParameterCode ?? r.ParameterId.ToString();
                },
                r =>
                {
                    var existingReading = updateReadingDict.GetValueOrDefault(r.Id!.Value);
                    return existingReading?.ParameterType ?? ParameterType.Cumulative;
                });

            if (!updateValidationResult.IsSuccess)
            {
                return updateValidationResult;
            }
        }

        // Validate readings mới
        if (readingsToAdd.Count > 0)
        {
            // Cập nhật previousValues với các giá trị từ readings được update
            var updatedValuesByParameter = new Dictionary<(Guid AssetId, Guid ParameterId), decimal>(previousValuesByParameter);

            foreach (var updateReading in readingsToUpdate)
            {
                var existingReading = updateReadingDict.GetValueOrDefault(updateReading.Id!.Value);
                if (existingReading != null)
                {
                    var key = (existingReading.AssetId, existingReading.ParameterId);
                    // Chỉ cập nhật nếu đây là reading cuối cùng của parameter đó
                    var lastReading = lastReadingsByParameter.GetValueOrDefault(key);
                    if (lastReading?.Id == existingReading.Id)
                    {
                        updatedValuesByParameter[key] = updateReading.Value;
                    }
                }
            }

            var addValidationResult = _readingValueValidator.ValidateBatchMonotonicProgression(
                readingsToAdd,
                updatedValuesByParameter,
                r => r.AssetId,
                r => r.ParameterId,
                r => r.Value,
                r => context.AssetDict.GetValueOrDefault(r.AssetId)?.AssetCode ?? r.AssetId.ToString(),
                r => assetParameterDict.GetValueOrDefault((r.AssetId, r.ParameterId))?.ParameterCode ?? r.ParameterId.ToString(),
                r => assetParameterDict.GetValueOrDefault((r.AssetId, r.ParameterId))?.Type ?? ParameterType.Cumulative);

            if (!addValidationResult.IsSuccess)
            {
                return addValidationResult;
            }
        }

        foreach (var reading in readings)
        {
            if (reading.Id.HasValue)
            {
                shiftLog.UpdateReadingValue(reading.Id.Value, reading.Value);
            }
            else
            {
                // New reading, add it
                var asset = context.AssetDict.GetValueOrDefault(reading.AssetId);
                if (asset == null)
                {
                    return Result.Invalid($"Asset with ID {reading.AssetId} does not exist");
                }

                var parameter = assetParameterDict.GetValueOrDefault((reading.AssetId, reading.ParameterId));

                if (parameter == null)
                {
                    return Result.Invalid($"Parameter with ID {reading.ParameterId} is not associated with Asset ID {reading.AssetId}");
                }

                shiftLog.AddReading(
                    assetId: reading.AssetId,
                    assetCode: asset.AssetCode,
                    assetName: asset.AssetName,
                    parameterId: reading.ParameterId,
                    parameterName: parameter.ParameterName,
                    parameterCode: parameter.ParameterCode,
                    parameterType: parameter.Type,
                    unitOfMeasureId: parameter.UnitOfMeasureId,
                    value: reading.Value,
                    readingTime: _clock.Now,
                    groupNumber: nextNumber++,
                    shiftLogCheckPointLinkedId: reading.CheckpointLinkedId);
            }
        }

        return Result.Success();
    }

}
