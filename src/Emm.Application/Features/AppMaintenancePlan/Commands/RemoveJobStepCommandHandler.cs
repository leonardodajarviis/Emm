using Emm.Application.Abstractions;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public class RemoveJobStepCommandHandler : IRequestHandler<RemoveJobStepCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenancePlanDefinitionRepository _repository;

    public RemoveJobStepCommandHandler(
        IUnitOfWork unitOfWork,
        IMaintenancePlanDefinitionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(RemoveJobStepCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var maintenancePlan = await _repository.GetByIdAsync(request.MaintenancePlanDefinitionId, cancellationToken);

            if (maintenancePlan == null)
            {
                return Result.Failure(ErrorType.NotFound,
                    $"Maintenance plan with ID {request.MaintenancePlanDefinitionId} not found.");
            }

            maintenancePlan.RemoveJobStep(request.JobStepId);

            _repository.Update(maintenancePlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        });
    }
}
