using Emm.Application.Services;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class AddShiftLogReadingsHandler : ICreateShiftLogBuilderHandler
{
    private readonly IQueryContext _queryContext;
    private readonly IReadingValueValidator _readingValueValidator;
    private readonly IDateTimeProvider _clock;

    public AddShiftLogReadingsHandler(
        IQueryContext queryContext,
        IReadingValueValidator readingValueValidator,
        IDateTimeProvider clock)
    {
        _queryContext = queryContext;
        _readingValueValidator = readingValueValidator;
        _clock = clock;
    }
    public async Task<Result> Handle(CreateShiftLogContext context, CancellationToken cancellationToken)
    {
        var shiftLog = context.ShiftLog;
        var data = context.Data;
        var assetIds = context.AssetDict.Keys.ToArray();

        var assetParameterDict = await _queryContext.Query<AssetParameter>()
            .Where(ap => assetIds.Contains(ap.AssetId))
            .ToDictionaryAsync(ap => (ap.AssetId, ap.ParameterId), ap => ap, cancellationToken);

        // Lấy reading cuối cùng của từng parameter theo groupNumber
        var lastReadingsByParameter = await _queryContext.Query<ShiftLogParameterReading>()
            .Where(r => r.OperationShiftId == shiftLog.OperationShiftId)
            .GroupBy(r => new { r.AssetId, r.ParameterId })
            .Select(g => new
            {
                g.Key.AssetId,
                g.Key.ParameterId,
                LastReading = g.OrderByDescending(r => r.GroupNumber).FirstOrDefault()
            })
            .ToDictionaryAsync(
                x => (x.AssetId, x.ParameterId),
                x => x.LastReading,
                cancellationToken);

        // Lấy groupNumber tiếp theo
        var maxGroupNumber = await _queryContext.Query<ShiftLogParameterReading>()
            .Where(r => r.OperationShiftId == shiftLog.OperationShiftId)
            .Select(r => (int?)r.GroupNumber)
            .MaxAsync(cancellationToken);

        var nextNumber = (maxGroupNumber ?? 0) + 1;

        // Chuẩn bị dictionary cho validation
        var previousValuesByParameter = new Dictionary<(Guid AssetId, Guid ParameterId), decimal>();
        if (nextNumber > 1)
        {
            previousValuesByParameter = lastReadingsByParameter
                .Where(x => x.Value != null && x.Value.ParameterType == ParameterType.Snapshot)
                .ToDictionary(x => x.Key, x => x.Value!.Value);
        }
        else
        {
            // Nếu là batch đầu tiên, tất cả giá trị trước đều là null
            previousValuesByParameter = await  _queryContext.Query<OperationShiftReadingSnapshot>()
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

        // Validate tính tịnh tiến của toàn bộ batch readings
        var validationResult = _readingValueValidator.ValidateBatchMonotonicProgression(
            data.Readings,
            previousValuesByParameter,
            r => r.AssetId,
            r => r.ParameterId,
            r => r.Value,
            r => context.AssetDict.GetValueOrDefault(r.AssetId)?.AssetCode ?? r.AssetId.ToString(),
            r => assetParameterDict.GetValueOrDefault((r.AssetId, r.ParameterId))?.ParameterCode ?? r.ParameterId.ToString(),
            r => assetParameterDict.GetValueOrDefault((r.AssetId, r.ParameterId))?.Type ?? ParameterType.Cumulative);

        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        foreach (var reading in data.Readings)
        {
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

        return Result.Success();
    }
}
