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
        var bodyRequest = request.Body;
        var assetModel = await _repository.GetByIdAsync(request.AssetModelId, cancellationToken);
        if (assetModel == null)
        {
            return Result<object>.Failure(ErrorType.NotFound, "Asset model not found.");
        }

        // Convert job steps if provided
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null;
        if (bodyRequest.JobSteps != null && bodyRequest.JobSteps.Count > 0)
        {
            jobSteps = [.. bodyRequest.JobSteps.Select(js => new MaintenancePlanJobStepDefinitionSpec(
                Name: js.Name,
                OrganizationUnitId: js.OrganizationUnitId,
                Note: js.Note,
                Order: js.Order
            ))];
        }

        // Add maintenance plan based on plan type
        switch (bodyRequest.PlanType)
        {
            case MaintenancePlanType.TimeBased:
                if (string.IsNullOrWhiteSpace(bodyRequest.RRule))
                {
                    return Result<object>.Failure(
                        ErrorType.Validation,
                        "RRule is required for time-based maintenance plans.");
                }

                assetModel.AddTimeBasedMaintenancePlan(
                    name: bodyRequest.Name,
                    description: bodyRequest.Description,
                    rrule: bodyRequest.RRule,
                    jobSteps: jobSteps,
                    isActive: bodyRequest.IsActive
                );
                break;

            case MaintenancePlanType.ParameterBased:
                if (!bodyRequest.ParameterId.HasValue ||
                    !bodyRequest.TriggerValue.HasValue ||
                    !bodyRequest.MinValue.HasValue ||
                    !bodyRequest.MaxValue.HasValue)
                {
                    return Result<object>.Failure(
                        ErrorType.Validation,
                        "ParameterId, TriggerValue, MinValue, MaxValue, and TriggerCondition are required for parameter-based maintenance plans.");
                }

                assetModel.AddParameterBasedMaintenancePlan(
                    name: bodyRequest.Name,
                    description: bodyRequest.Description,
                    parameterId: bodyRequest.ParameterId.Value,
                    triggerValue: bodyRequest.TriggerValue.Value,
                    minValue: bodyRequest.MinValue.Value,
                    maxValue: bodyRequest.MaxValue.Value,
                    triggerCondition: MaintenanceTriggerCondition.Equal,
                    jobSteps: jobSteps,
                    isActive: bodyRequest.IsActive
                );
                break;

            default:
                return Result<object>.Failure(ErrorType.Validation, "Invalid maintenance plan type.");
        }

        // Get the newly added maintenance plan (last one in collection) to add required items
        var maintenancePlan = assetModel.MaintenancePlanDefinitions.LastOrDefault();

        // Add required items if provided
        if (bodyRequest.RequiredItems != null && bodyRequest.RequiredItems.Count > 0 && maintenancePlan != null)
        {
            foreach (var item in bodyRequest.RequiredItems)
            {
                assetModel.AddRequiredItemToMaintenancePlan(
                    maintenancePlanId: maintenancePlan.Id,
                    itemGroupId: item.ItemGroupId,
                    itemId: item.ItemId,
                    unitOfMeasureId: item.UnitOfMeasureId,
                    quantity: item.Quantity,
                    isRequired: item.IsRequired,
                    note: item.Note
                );
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            request.AssetModelId,
        });
    }
}
