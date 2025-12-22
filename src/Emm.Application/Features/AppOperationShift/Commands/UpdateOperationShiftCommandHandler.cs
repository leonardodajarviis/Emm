using Emm.Application.ErrorCodes;
using Emm.Domain.Entities.Operations;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class UpdateOperationShiftCommandHandler : IRequestHandler<UpdateOperationShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, Guid> _repository;

    public UpdateOperationShiftCommandHandler(IUnitOfWork unitOfWork, IRepository<OperationShift, Guid> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(UpdateOperationShiftCommand request, CancellationToken cancellationToken)
    {
        var operationShift = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (operationShift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        operationShift.Update(
            name: request.Name,
            description: request.Description,
            locationId: request.LocationId,
            scheduledStartTime: request.ScheduledStartTime,
            scheduledEndTime: request.ScheduledEndTime
        );

        if (!string.IsNullOrEmpty(request.Notes))
        {
            operationShift.UpdateNotes(request.Notes);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
