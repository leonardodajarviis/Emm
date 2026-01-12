using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public class CreateMaintenancePlanDefinitionCommandHandler : IRequestHandler<CreateMaintenancePlanDefinitionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenancePlanDefinitionRepository _repository;
    private readonly IQueryContext _queryContext;
    private readonly IUserContextService _userContextService;

    public CreateMaintenancePlanDefinitionCommandHandler(
        IUnitOfWork unitOfWork,
        IMaintenancePlanDefinitionRepository repository,
        IQueryContext queryContext,
        IUserContextService userContextService)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(queryContext);
        ArgumentNullException.ThrowIfNull(userContextService);

        _unitOfWork = unitOfWork;
        _repository = repository;
        _queryContext = queryContext;
        _userContextService = userContextService;
    }

    public async Task<Result> Handle(CreateMaintenancePlanDefinitionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            // Validate asset model exists
            var assetModelExists = await _queryContext.Query<AssetModel>()
                .AnyAsync(am => am.Id == request.AssetModelId, cancellationToken);

            if (!assetModelExists)
            {
                return Result.Failure(ErrorType.NotFound, $"Asset model with ID {request.AssetModelId} not found.");
            }

            // Check if maintenance plan with same name already exists for this asset model
            var nameExists = await _repository.ExistsByNameAndAssetModelIdAsync(
                request.Name,
                request.AssetModelId,
                cancellationToken);

            if (nameExists)
            {
                return Result.Failure(ErrorType.Conflict,
                    $"Maintenance plan with name '{request.Name}' already exists for this asset model.");
            }

            MaintenancePlanDefinition maintenancePlan;

            // Create maintenance plan based on type
            if (request.PlanType == MaintenancePlanType.TimeBased)
            {
                if (string.IsNullOrWhiteSpace(request.RRule))
                {
                    return Result.Failure(ErrorType.Validation, "RRule is required for time-based maintenance plans.");
                }

                maintenancePlan = new MaintenancePlanDefinition(
                    assetModelId: request.AssetModelId,
                    name: request.Name,
                    description: request.Description,
                    rrule: request.RRule,
                    isActive: request.IsActive
                );
            }
            else if (request.PlanType == MaintenancePlanType.ParameterBased)
            {
                if (!request.ParameterId.HasValue || !request.Value.HasValue ||
                    !request.PlusTolerance.HasValue || !request.MinusTolerance.HasValue)
                {
                    return Result.Failure(ErrorType.Validation,
                        "ParameterId, Value, PlusTolerance, and MinusTolerance are required for parameter-based maintenance plans.");
                }

                maintenancePlan = new MaintenancePlanDefinition(
                    assetModelId: request.AssetModelId,
                    name: request.Name,
                    description: request.Description,
                    parameterId: request.ParameterId.Value,
                    value: request.Value.Value,
                    plusTolerance: request.PlusTolerance.Value,
                    minusTolerance: request.MinusTolerance.Value,
                    isActive: request.IsActive
                );
            }
            else
            {
                return Result.Failure(ErrorType.Validation, $"Invalid maintenance plan type: {request.PlanType}");
            }

            // Add job steps if provided
            if (request.JobSteps?.Count > 0)
            {
                foreach (var jobStep in request.JobSteps)
                {
                    maintenancePlan.AddJobStep(
                        name: jobStep.Name,
                        organizationUnitId: jobStep.OrganizationUnitId,
                        note: jobStep.Note,
                        order: jobStep.Order
                    );
                }
            }

            // Add required items if provided
            if (request.RequiredItems?.Count > 0)
            {
                foreach (var item in request.RequiredItems)
                {
                    maintenancePlan.AddRequiredItem(
                        itemGroupId: item.ItemGroupId,
                        itemId: item.ItemId,
                        unitOfMeasureId: item.UnitOfMeasureId,
                        quantity: item.Quantity,
                        isRequired: item.IsRequired,
                        note: item.Note
                    );
                }
            }

            await _repository.AddAsync(maintenancePlan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new { maintenancePlan.Id });
        });
    }
}
