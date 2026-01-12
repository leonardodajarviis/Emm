namespace Emm.Application.Features.AppMaintenancePlan.Commands;

public class AddRequiredItemCommandHandler : IRequestHandler<AddRequiredItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMaintenancePlanDefinitionRepository _repository;

    public AddRequiredItemCommandHandler(
        IUnitOfWork unitOfWork,
        IMaintenancePlanDefinitionRepository repository)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        ArgumentNullException.ThrowIfNull(repository);

        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(AddRequiredItemCommand request, CancellationToken cancellationToken)
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

            maintenancePlan.AddRequiredItem(
                itemGroupId: request.ItemGroupId,
                itemId: request.ItemId,
                unitOfMeasureId: request.UnitOfMeasureId,
                quantity: request.Quantity,
                isRequired: request.IsRequired,
                note: request.Note
            );

            _repository.Update(maintenancePlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        });
    }
}
