using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Application.Common.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class UpdateAssetBoxCommandHandler : IRequestHandler<UpdateAssetBoxCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAssetBoxCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(UpdateAssetBoxCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.UpdateAssetBox(
            request.AssetBoxId,
            request.BoxName,
            request.Role,
            request.DisplayOrder,
            request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Asset group updated successfully");
    }
}
