namespace Emm.Application.Features.AppAssetModel.Commands;

public class RemoveRequiredItemFromMaintenancePlanCommandHandler : IRequestHandler<RemoveRequiredItemFromMaintenancePlanCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;

    public RemoveRequiredItemFromMaintenancePlanCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(RemoveRequiredItemFromMaintenancePlanCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        assetModel.RemoveRequiredItemFromMaintenancePlan(
            maintenancePlanId: request.MaintenancePlanId,
            requiredItemId: request.RequiredItemId
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            AssetModelId = request.AssetModelId,
            MaintenancePlanId = request.MaintenancePlanId,
            Message = "Required item removed successfully"
        });
    }
}
