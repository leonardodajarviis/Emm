using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.Operations;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CancelShiftCommandHandler : IRequestHandler<CancelShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, long> _repository;
    private readonly IUserContextService _userContextService;

    public CancelShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<OperationShift, long> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(CancelShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var currentUser = _userContextService.GetCurrentUsername() ?? "System";
        var reasonWithUser = $"{request.Reason} (Cancelled by: {currentUser})";

        shift.CancelShift(reasonWithUser);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new {
            ShiftId = shift.Id,
            Status = shift.Status.ToString(),
            Reason = reasonWithUser,
            CancelledBy = currentUser,
            CancelledAt = DateTime.UtcNow
        });
    }
}
