using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;
using Emm.Domain.Abstractions;

namespace  Emm.Application.Features.AppOperationShift.Commands;

public class StartOperationShiftCommandHandler : IRequestHandler<StartOperationShiftCommand, Result<object>>
{
    private readonly IOperationShiftRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _clock;

    public StartOperationShiftCommandHandler(
        IOperationShiftRepository repository,
        IDateTimeProvider clock,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<Result<object>> Handle(StartOperationShiftCommand request, CancellationToken cancellationToken)
    {
        var operationShift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (operationShift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        operationShift.StartShift(actualStartTime: _clock.Now);

        _repository.Update(operationShift);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            ShiftId = operationShift.Id,
            Status = operationShift.Status.ToString(),
            operationShift.ActualStartTime
        });
    }
}
