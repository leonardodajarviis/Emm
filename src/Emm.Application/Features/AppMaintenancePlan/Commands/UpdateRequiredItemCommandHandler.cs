namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public class UpdateRequiredItemCommandHandler : IRequestHandler<UpdateRequiredItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenancePlanDefinitionRepository _repository;

    public UpdateRequiredItemCommandHandler(
        IUnitOfWork unitOfWork,
        IMaintenancePlanDefinitionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateRequiredItemCommand request, CancellationToken cancellationToken)
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

            try
            {
                maintenancePlan.UpdateRequiredItem(
                    requiredItemId: request.RequiredItemId,
                    quantity: request.Quantity,
                    isRequired: request.IsRequired,
                    note: request.Note
                );

                _repository.Update(maintenancePlan);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ErrorType.NotFound, ex.Message);
            }
        });
    }
}
