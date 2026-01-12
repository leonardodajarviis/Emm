using Emm.Application.Abstractions;
using Emm.Domain.Repositories;

namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public class DeleteMaintenancePlanDefinitionCommandHandler : IRequestHandler<DeleteMaintenancePlanDefinitionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenancePlanDefinitionRepository _repository;

    public DeleteMaintenancePlanDefinitionCommandHandler(
        IUnitOfWork unitOfWork,
        IMaintenancePlanDefinitionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteMaintenancePlanDefinitionCommand request, CancellationToken cancellationToken)
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

            _repository.Remove(maintenancePlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        });
    }
}
