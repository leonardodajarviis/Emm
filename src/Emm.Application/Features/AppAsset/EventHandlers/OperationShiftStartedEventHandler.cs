using Emm.Domain.Events.Operations;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class OperationShiftStartedEventHandler : IEventHandler<OperationShiftStartedEvent>
{
    private readonly IAssetRepository _assetRepository;

    public OperationShiftStartedEventHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task Handle(OperationShiftStartedEvent @event, CancellationToken cancellationToken)
    {
        await _assetRepository.OperateMultiAsync(@event.AssetIds, cancellationToken);
    }
}
