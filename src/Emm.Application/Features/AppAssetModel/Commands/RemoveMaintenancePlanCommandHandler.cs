using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Services;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class RemoveMaintenancePlanCommandHandler : IRequestHandler<RemoveMaintenancePlanCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;
    private readonly MaintenancePlanManagementService _maintenancePlanService;

    public RemoveMaintenancePlanCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository,
        MaintenancePlanManagementService maintenancePlanService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _maintenancePlanService = maintenancePlanService;
    }

    public async Task<Result<object>> Handle(RemoveMaintenancePlanCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        _maintenancePlanService.RemoveMaintenancePlan(assetModel, request.MaintenancePlanId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            AssetModelId = request.AssetModelId,
            MaintenancePlanId = request.MaintenancePlanId,
            Message = "Maintenance plan removed successfully"
        });
    }
}
