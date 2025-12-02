using Emm.Domain.Events.Operations;

namespace Emm.Application.Features.AppAsset.EventHandlers;

public class OperationShiftStartedEventHandler : IEventHandler<OperationShiftStartedEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetRepository _assetRepository;

    public OperationShiftStartedEventHandler(IUnitOfWork unitOfWork, IAssetRepository assetRepository)
    {
        _unitOfWork = unitOfWork;
        _assetRepository = assetRepository;
    }

    public async Task Handle(OperationShiftStartedEvent @event, CancellationToken cancellationToken)
    {
        await _assetRepository.OperateMultiAsync(@event.AssetIds, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
