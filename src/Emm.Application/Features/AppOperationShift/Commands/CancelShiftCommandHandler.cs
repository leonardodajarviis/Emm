namespace Emm.Application.Features.AppOperationShift.Commands;

public class CancelShiftCommandHandler : IRequestHandler<CancelShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;

    public CancelShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository
        )
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result> Handle(CancelShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result.NotFound("Không tìm thấy ca vận hành");
        }

        shift.CancelShift(request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
