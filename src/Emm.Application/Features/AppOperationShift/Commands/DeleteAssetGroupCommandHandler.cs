using Emm.Application.Common.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class DeleteAssetGroupCommandHandler : IRequestHandler<DeleteAssetGroupCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAssetGroupCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(DeleteAssetGroupCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.RemoveAssetGroup(request.AssetGroupId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Asset group deleted successfully");
    }
}
