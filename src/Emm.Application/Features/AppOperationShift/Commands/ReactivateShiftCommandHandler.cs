using Emm.Application.Abstractions;
using Emm.Application.ErrorCodes;
using Emm.Domain.Entities.Operations;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class ReactivateShiftCommandHandler : IRequestHandler<ReactivateShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, Guid> _repository;
    private readonly IUserContextService _userContextService;

    public ReactivateShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<OperationShift, Guid> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(ReactivateShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var currentUser = _userContextService.GetCurrentUsername() ?? "System";
        var reasonWithUser = $"{request.Reason} (Reactivated by: {currentUser})";

        shift.Reactivate(
            request.NewScheduledStartTime,
            request.NewScheduledEndTime,
            reasonWithUser);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            ShiftId = shift.Id,
            Status = shift.Status.ToString(),
            NewScheduledStartTime = shift.ScheduledStartTime,
            NewScheduledEndTime = shift.ScheduledEndTime,
            Reason = reasonWithUser,
            ReactivatedBy = currentUser,
            ReactivatedAt = DateTime.UtcNow
        });
    }
}
