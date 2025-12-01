using Emm.Application.Abstractions;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CompleteShiftCommandHandler : IRequestHandler<CompleteShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, long> _repository;
    private readonly IUserContextService _userContextService;

    public CompleteShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<OperationShift, long> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(CompleteShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.CompleteShift(request.ActualEndTime, request.Notes);

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
