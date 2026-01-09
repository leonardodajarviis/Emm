using Emm.Domain.Entities.AssetCatalog;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class SyncShiftLogReadingsHandler : IUpdateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;
    public SyncShiftLogReadingsHandler(IQueryContext queryContext)
    {
        _queryContext = queryContext;
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
                    return Result.Validation($"Asset with ID {reading.AssetId} does not exist");
                }

                var parameter = assetParameterDict.GetValueOrDefault((reading.AssetId, reading.ParameterId));

                if (parameter == null)
                {
                    return Result.Validation($"Parameter with ID {reading.ParameterId} is not associated with Asset ID {reading.AssetId}");
                }

                shiftLog.AddReading(
                    assetId: reading.AssetId,
                    assetCode: asset.AssetCode,
                    assetName: asset.AssetName,
                    parameterId: reading.ParameterId,
                    parameterName: parameter.ParameterName,
                    parameterCode: parameter.ParameterCode,
                    unitOfMeasureId: parameter.UnitOfMeasureId,
                    value: reading.Value,
                    shiftLogCheckPointLinkedId: reading.CheckpointLinkedId);
            }
        }

        return Result.Success();
    }

}
