using Emm.Application.Common.ErrorCodes;
using Emm.Domain.Entities.Operations;
using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class DeleteOperationShiftCommandHandler : IRequestHandler<DeleteOperationShiftCommand, Result<object>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<OperationShift, long> _repository;

    public DeleteOperationShiftCommandHandler(IUnitOfWork unitOfWork, IRepository<OperationShift, long> repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<Result<object>> Handle(DeleteOperationShiftCommand request, CancellationToken cancellationToken)
    {
        var operationShift = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (operationShift == null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        _repository.Remove(operationShift);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success(new
        {
            Id = request.Id
        });
    }
}
