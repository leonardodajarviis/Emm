namespace Emm.Application.Features.AppAssetModel.Commands;

public class AddRequiredItemToMaintenancePlanCommandHandler : IRequestHandler<AddRequiredItemToMaintenancePlanCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;

    public AddRequiredItemToMaintenancePlanCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(AddRequiredItemToMaintenancePlanCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        assetModel.AddRequiredItemToMaintenancePlan(
            maintenancePlanId: request.MaintenancePlanId,
            itemId: request.ItemId,
            quantity: request.Quantity,
            isRequired: request.IsRequired,
            note: request.Note
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            AssetModelId = request.AssetModelId,
            MaintenancePlanId = request.MaintenancePlanId,
            Message = "Required item added successfully"
        });
    }
}
