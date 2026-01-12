using Emm.Application.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public class UpdateMaintenancePlanDefinitionCommandHandler : IRequestHandler<UpdateMaintenancePlanDefinitionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenancePlanDefinitionRepository _repository;

    public UpdateMaintenancePlanDefinitionCommandHandler(
        IUnitOfWork unitOfWork,
        IMaintenancePlanDefinitionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateMaintenancePlanDefinitionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var maintenancePlan = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (maintenancePlan == null)
            {
                return Result.Failure(ErrorType.NotFound,
                    $"Maintenance plan with ID {request.Id} not found.");
            }

            // Update based on plan type
            if (maintenancePlan.PlanType == MaintenancePlanType.TimeBased)
            {
                if (string.IsNullOrWhiteSpace(request.RRule))
                {
                    return Result.Failure(ErrorType.Validation,
                        "RRule is required for time-based maintenance plans.");
                }

                maintenancePlan.UpdateTimeBasedPlan(
                    name: request.Name,
                    description: request.Description,
                    rrule: request.RRule,
                    isActive: request.IsActive
                );
            }
            else if (maintenancePlan.PlanType == MaintenancePlanType.ParameterBased)
            {
                if (!request.Value.HasValue || !request.PlusTolerance.HasValue || !request.MinusTolerance.HasValue)
                {
                    return Result.Failure(ErrorType.Validation,
                        "Value, PlusTolerance, and MinusTolerance are required for parameter-based maintenance plans.");
                }

                maintenancePlan.UpdateParameterBasedPlan(
                    name: request.Name,
                    description: request.Description,
                    thresholdValue: request.Value.Value,
                    plusTolerance: request.PlusTolerance.Value,
                    minusTolerance: request.MinusTolerance.Value,
                    isActive: request.IsActive
                );
            }

            _repository.Update(maintenancePlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new { maintenancePlan.Id });
        });
    }
}
