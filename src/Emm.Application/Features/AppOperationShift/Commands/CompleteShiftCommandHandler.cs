using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOperationShiftRepository _repository;
    private readonly IShiftLogRepository _shiftLogRepository;
    private readonly IUserContextService _userContextService;

    public CompleteShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IOperationShiftRepository repository,
        IUserContextService userContextService,
        IShiftLogRepository shiftLogRepository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
        _shiftLogRepository = shiftLogRepository;
    }

    public async Task<Result<object>> Handle(CompleteShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var shiftLogs = await _shiftLogRepository.GetByShiftIdAsync(request.ShiftId, cancellationToken);

        foreach (var log in shiftLogs)
        {
            log.LockAllReadings();
        }

        shift.CompleteShift(DateTime.UtcNow, "noProplem");

        // Add log with current user info
        var currentUser = _userContextService.GetCurrentUsername() ?? "System";
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new {
            ShiftId = shift.Id,
            Status = shift.Status.ToString(),
            ActualEndTime = shift.ActualEndTime,
            CompletedBy = currentUser
        });
    }
}
