using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Application.Common.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AssignAssetToGroupCommandHandler : IRequestHandler<AssignAssetToGroupCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignAssetToGroupCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(AssignAssetToGroupCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.AssignAssetToGroup(request.AssetId, request.AssetGroupId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Asset assigned to group successfully");
    }
}
