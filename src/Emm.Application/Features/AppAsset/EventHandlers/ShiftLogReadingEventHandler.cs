
using Emm.Domain.Events.ShiftLogs;

namespace Emm.Application.Features.AppAsset.EventHandlers;


public class ShiftLogReadingEventHandler : IEventHandler<ShiftLogReadingEvent>
{
    private readonly IAssetRepository _assetRepository;
    public ShiftLogReadingEventHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task Handle(ShiftLogReadingEvent @event, CancellationToken cancellationToken = default)
    {
        var assetIds = @event.ParameterReadings
            .Select(line => line.AssetId)
            .Distinct()
            .ToList();

        var assets = await _assetRepository.GetMultiByIdsAsync(assetIds, cancellationToken);

        foreach (var readings in @event.ParameterReadings.GroupBy(r => r.AssetId))
        {
            var asset = assets.FirstOrDefault(a => a.Id == readings.Key);
            if (asset == null)
                continue;
            foreach (var reading in readings)
            {
                asset.RecordParameter(reading.ParameterId, reading.Value);
            }
        }
    }
}
