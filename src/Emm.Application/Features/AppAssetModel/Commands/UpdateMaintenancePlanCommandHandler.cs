using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Services;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class UpdateMaintenancePlanCommandHandler : IRequestHandler<UpdateMaintenancePlanCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;
    private readonly MaintenancePlanManagementService _maintenancePlanService;

    public UpdateMaintenancePlanCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository,
        MaintenancePlanManagementService maintenancePlanService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _maintenancePlanService = maintenancePlanService;
    }

    public async Task<Result<object>> Handle(UpdateMaintenancePlanCommand request, CancellationToken cancellationToken)
    {
        var bodyRequest = request.Body;
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        // Update based on plan type
        switch (bodyRequest.PlanType)
        {
            case MaintenancePlanType.TimeBased:
                if (string.IsNullOrWhiteSpace(bodyRequest.RRule))
                {
                    return Result<object>.Failure(ErrorType.Validation, "RRule is required for time-based maintenance plans.");
                }

                _maintenancePlanService.UpdateTimeBasedMaintenancePlan(
                    assetModel: assetModel,
                    maintenancePlanId: request.MaintenancePlanId,
                    name: bodyRequest.Name,
                    description: bodyRequest.Description,
                    rrule: bodyRequest.RRule,
                    isActive: bodyRequest.IsActive
                );
                break;

            case MaintenancePlanType.ParameterBased:
                if (!bodyRequest.TriggerValue.HasValue ||
                    !bodyRequest.MinusTolerance.HasValue ||
                    !bodyRequest.PlusTolerance.HasValue)
                {
                    return Result<object>.Failure(
                        ErrorType.Validation,
                        "TriggerValue, MinusTolerance, PlusTolerance, and TriggerCondition are required for parameter-based maintenance plans.");
                }

                _maintenancePlanService.UpdateParameterBasedMaintenancePlan(
                    assetModel: assetModel,
                    maintenancePlanId: request.MaintenancePlanId,
                    name: bodyRequest.Name,
                    description: bodyRequest.Description,
                    triggerValue: bodyRequest.TriggerValue.Value,
                    minusTolerance: bodyRequest.MinusTolerance.Value,
                    plusTolerance: bodyRequest.PlusTolerance.Value,
                    triggerCondition: MaintenanceTriggerCondition.Equal,
                    isActive: bodyRequest.IsActive
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
        if (bodyRequest.JobSteps != null && bodyRequest.JobSteps.Count > 0)
        {
            var jobStepSpecs = bodyRequest.JobSteps.Select(js => new MaintenancePlanJobStepDefinitionSpec(
                Id: js.Id,
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order
            )).ToList();

            maintenancePlan.SyncJobSteps(jobStepSpecs);
        }

        // Sync required items if provided
        if (bodyRequest.RequiredItems != null && bodyRequest.RequiredItems.Count > 0)
        {
            var requiredItemSpecs = bodyRequest.RequiredItems.Select(ri => new MaintenancePlanRequiredItemDefinitionSpec(
                Id: ri.Id,
                ItemGroupId: ri.ItemGroupId,
                ItemId: ri.ItemId,
                UnitOfMeasureId: ri.UnitOfMeasureId,
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
