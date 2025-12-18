using Emm.Application.Abstractions;
using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.Operations;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class ResumeShiftCommandHandler : IRequestHandler<ResumeShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, Guid> _repository;
    private readonly IUserContextService _userContextService;

    public ResumeShiftCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<OperationShift, Guid> repository,
        IUserContextService userContextService)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _userContextService = userContextService;
    }

    public async Task<Result<object>> Handle(ResumeShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await _repository.GetByIdAsync(request.ShiftId, cancellationToken);
        if (shift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        var currentUser = _userContextService.GetCurrentUsername() ?? "System";
        var notes = string.IsNullOrEmpty(request.Notes)
            ? $"Resumed by: {currentUser}"
            : $"{request.Notes} (Resumed by: {currentUser})";

        shift.ResumeShift(notes);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            ShiftId = shift.Id,
            Status = shift.Status.ToString(),
            ResumedBy = currentUser,
            ResumedAt = DateTime.UtcNow
        });
    }
}
