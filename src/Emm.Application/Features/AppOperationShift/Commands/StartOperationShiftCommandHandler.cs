using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;

namespace  Emm.Application.Features.AppOperationShift.Commands;

public class StartOperationShiftCommandHandler : IRequestHandler<StartOperationShiftCommand, Result<object>>
{
    private readonly IOperationShiftRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutbox _outbox;

    public StartOperationShiftCommandHandler(
        IOperationShiftRepository repository,
        IUnitOfWork unitOfWork,
        IOutbox outbox)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outbox = outbox;
    }

    public async Task<Result<object>> Handle(StartOperationShiftCommand request, CancellationToken cancellationToken)
    {
        var operationShift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (operationShift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        operationShift.StartShift(DateTime.UtcNow);

        _repository.Update(operationShift);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            ShiftId = operationShift.Id,
            Status = operationShift.Status.ToString(),
            ActualStartTime = operationShift.ActualStartTime
        });
    }
}