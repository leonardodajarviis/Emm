using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Application.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AssignAssetToBoxCommandHandler : IRequestHandler<AssignAssetToBoxCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignAssetToBoxCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(AssignAssetToBoxCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.AssignAssetToBox(request.AssetId, request.AssetBoxId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Asset assigned to box successfully");
    }
}
