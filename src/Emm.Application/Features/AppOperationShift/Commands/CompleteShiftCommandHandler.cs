using Emm.Domain.Abstractions;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IShiftLogRepository _shiftLogRepository;
    private readonly IDateTimeProvider _clock;

    public CompleteShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IShiftLogRepository shiftLogRepository,
        IDateTimeProvider clock)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _shiftLogRepository = shiftLogRepository;
        _clock = clock;
    }

    public async Task<Result> Handle(CompleteShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result.NotFound("Operation shift not found");
        }

        var shiftLogs = await _shiftLogRepository.GetByShiftIdAsync(request.ShiftId, cancellationToken);

        foreach (var log in shiftLogs)
        {
            log.LockAllReadings();
        }

        shift.CompleteShift(_clock.Now, "noProplem");

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
