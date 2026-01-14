using Emm.Domain.Abstractions;

namespace  Emm.Application.Features.AppOperationShift.Commands;

public class StartOperationShiftCommandHandler : IRequestHandler<StartOperationShiftCommand, Result>
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

    public async Task<Result> Handle(StartOperationShiftCommand request, CancellationToken cancellationToken)
    {
        var operationShift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (operationShift == null)
        {
            return Result.NotFound("Không tìm thấy ca vận hành");
        }

        operationShift.StartShift(actualStartTime: _clock.Now);

        _repository.Update(operationShift);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
