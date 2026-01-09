using Emm.Domain.Events.Operations;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class OperationShiftCompletedEventHandler : IEventHandler<OperationShiftCompletedEvent>
{
    private readonly IAssetRepository _assetRepository;

    public OperationShiftCompletedEventHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task Handle(OperationShiftCompletedEvent @event, CancellationToken cancellationToken)
    {
        await _assetRepository.IdleMultiAsync(@event.AssetIds, cancellationToken);
    }
}
