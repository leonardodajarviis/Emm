using Emm.Domain.Events.Operations;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class OperationShiftCompletedEventHandler : IEventHandler<OperationShiftCompletedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetRepository _assetRepository;

    public OperationShiftCompletedEventHandler(IUnitOfWork unitOfWork, IAssetRepository assetRepository)
    {
        _unitOfWork = unitOfWork;
        _assetRepository = assetRepository;
    }

    public async Task Handle(OperationShiftCompletedEvent @event, CancellationToken cancellationToken)
    {
        await _assetRepository.IdleMultiAsync(@event.AssetIds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
