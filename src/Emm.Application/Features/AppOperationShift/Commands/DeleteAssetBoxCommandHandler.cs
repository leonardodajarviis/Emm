using Emm.Application.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class DeleteAssetBoxCommandHandler : IRequestHandler<DeleteAssetBoxCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAssetBoxCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(DeleteAssetBoxCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.RemoveAssetBox(request.AssetBoxId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Asset box deleted successfully");
    }
}
