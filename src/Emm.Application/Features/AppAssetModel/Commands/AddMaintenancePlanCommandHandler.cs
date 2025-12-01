using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Commands;

public class AddMaintenancePlanCommandHandler : IRequestHandler<AddMaintenancePlanCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAssetModelRepository _repository;

    public AddMaintenancePlanCommandHandler(
        IUnitOfWork unitOfWork,
        IAssetModelRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(AddMaintenancePlanCommand request, CancellationToken cancellationToken)
    {
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        // Convert job steps if provided
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null;
        if (request.JobSteps != null && request.JobSteps.Count > 0)
        {
            jobSteps = request.JobSteps.Select(js => new MaintenancePlanJobStepDefinitionSpec(
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order
            )).ToList();
        }

        // Add maintenance plan based on plan type
        switch (request.PlanType)
        {
            case MaintenancePlanType.TimeBased:
                if (string.IsNullOrWhiteSpace(request.RRule))
                {
                    return Result<object>.Failure(
                        ErrorType.Validation,
                        "RRule is required for time-based maintenance plans.");
                }

                assetModel.AddTimeBasedMaintenancePlan(
                    name: request.Name,
                    description: request.Description,
                    rrule: request.RRule,
                    jobSteps: jobSteps,
                    isActive: request.IsActive
                );
                break;

            case MaintenancePlanType.ParameterBased:
                if (!request.ParameterId.HasValue ||
                    !request.TriggerValue.HasValue ||
                    !request.MinValue.HasValue ||
                    !request.MaxValue.HasValue)
                {
                    return Result<object>.Failure(
                        ErrorType.Validation,
                        "ParameterId, TriggerValue, MinValue, MaxValue, and TriggerCondition are required for parameter-based maintenance plans.");
                }

                assetModel.AddParameterBasedMaintenancePlan(
                    name: request.Name,
                    description: request.Description,
                    parameterId: request.ParameterId.Value,
                    triggerValue: request.TriggerValue.Value,
                    minValue: request.MinValue.Value,
                    maxValue: request.MaxValue.Value,
                    triggerCondition: MaintenanceTriggerCondition.Equal,
                    jobSteps: jobSteps,
                    isActive: request.IsActive
                );
                break;

            default:
                return Result<object>.Failure(ErrorType.Validation, "Invalid maintenance plan type.");
        }

        // Get the newly added maintenance plan (last one in collection) to add required items
        var maintenancePlan = assetModel.MaintenancePlanDefinitions.LastOrDefault();

        // Add required items if provided
        if (request.RequiredItems != null && request.RequiredItems.Count > 0 && maintenancePlan != null)
        {
            foreach (var item in request.RequiredItems)
            {
                assetModel.AddRequiredItemToMaintenancePlan(
                    maintenancePlanId: maintenancePlan.Id,
                    itemId: item.ItemId,
                    quantity: item.Quantity,
                    isRequired: item.IsRequired,
                    note: item.Note
                );
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            AssetModelId = request.AssetModelId,
            Message = "Maintenance plan added successfully"
        });
    }
}
