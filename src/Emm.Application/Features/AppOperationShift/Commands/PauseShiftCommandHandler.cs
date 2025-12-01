using Emm.Application.Abstractions;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.Operations;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class PauseShiftCommandHandler : IRequestHandler<PauseShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, long> _repository;
    private readonly IUserContextService _userContextService;

    public PauseShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<OperationShift, long> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(PauseShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var currentUser = _userContextService.GetCurrentUsername() ?? "System";
        var reasonWithUser = $"{request.Reason} (Paused by: {currentUser})";

        shift.PauseShift(reasonWithUser);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            ShiftId = shift.Id,
            Status = shift.Status.ToString(),
            Reason = reasonWithUser,
            PausedBy = currentUser,
            PausedAt = DateTime.UtcNow
        });
    }
}
