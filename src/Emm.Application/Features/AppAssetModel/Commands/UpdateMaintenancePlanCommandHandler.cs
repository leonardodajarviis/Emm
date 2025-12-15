using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class UpdateMaintenancePlanCommandHandler : IRequestHandler<UpdateMaintenancePlanCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;

    public UpdateMaintenancePlanCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateMaintenancePlanCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        // Update based on plan type
        switch (request.PlanType)
        {
            case MaintenancePlanType.TimeBased:
                if (string.IsNullOrWhiteSpace(request.RRule))
                {
                    return Result<object>.Failure(ErrorType.Validation, "RRule is required for time-based maintenance plans.");
                }

                assetModel.UpdateTimeBasedMaintenancePlan(
                    maintenancePlanId: request.MaintenancePlanId,
                    name: request.Name,
                    description: request.Description,
                    rrule: request.RRule,
                    isActive: request.IsActive
                );
                break;

            case MaintenancePlanType.ParameterBased:
                if (!request.TriggerValue.HasValue ||
                    !request.MinValue.HasValue ||
                    !request.MaxValue.HasValue)
                {
                    return Result<object>.Failure(
                        ErrorType.Validation,
                        "TriggerValue, MinValue, MaxValue, and TriggerCondition are required for parameter-based maintenance plans.");
                }

                assetModel.UpdateParameterBasedMaintenancePlan(
                    maintenancePlanId: request.MaintenancePlanId,
                    name: request.Name,
                    description: request.Description,
                    triggerValue: request.TriggerValue.Value,
                    minValue: request.MinValue.Value,
                    maxValue: request.MaxValue.Value,
                    triggerCondition: MaintenanceTriggerCondition.Equal,
                    isActive: request.IsActive
                );
                break;

            default:
                return Result<object>.Failure(ErrorType.Validation, "Invalid maintenance plan type.");
        }

        // Get maintenance plan for syncing
        var maintenancePlan = assetModel.MaintenancePlanDefinitions
            .FirstOrDefault(mp => mp.Id == request.MaintenancePlanId);

        if (maintenancePlan == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Maintenance plan not found.");
        }

        // Sync job steps if provided
        if (request.JobSteps != null && request.JobSteps.Count > 0)
        {
            var jobStepSpecs = request.JobSteps.Select(js => new JobStepSpec(
                Id: js.Id,
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order
            )).ToList();

            maintenancePlan.SyncJobSteps(jobStepSpecs);
        }

        // Sync required items if provided
        if (request.RequiredItems != null && request.RequiredItems.Count > 0)
        {
            var requiredItemSpecs = request.RequiredItems.Select(ri => new RequiredItemSpec(
                Id: ri.Id,
                ItemId: ri.ItemId,
                Quantity: ri.Quantity,
                IsRequired: ri.IsRequired,
                Note: ri.Note
            )).ToList();

            maintenancePlan.SyncRequiredItems(requiredItemSpecs);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            request.AssetModelId,
            request.MaintenancePlanId
        });
    }
}
