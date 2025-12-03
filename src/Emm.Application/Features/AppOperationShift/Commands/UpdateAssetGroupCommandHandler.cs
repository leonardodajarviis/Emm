using Emm.Application.Abstractions;
using Emm.Application.Common;
using Emm.Application.Common.ErrorCodes;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class UpdateAssetGroupCommandHandler : IRequestHandler<UpdateAssetGroupCommand, Result<object>>
{
    private readonly IOperationShiftRepository _operationShiftRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAssetGroupCommandHandler(
        IOperationShiftRepository operationShiftRepository,
        IUnitOfWork unitOfWork)
    {
        _operationShiftRepository = operationShiftRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<object>> Handle(UpdateAssetGroupCommand request, CancellationToken cancellationToken)
    {
        var shift = await _operationShiftRepository.GetByIdAsync(request.OperationShiftId, cancellationToken);
        if (shift is null)
        {
            return Result<object>.NotFound("Operation shift not found", ShiftErrorCodes.NotFound);
        }

        shift.UpdateAssetGroup(
            request.AssetGroupId,
            request.GroupName,
            request.Role,
            request.DisplayOrder,
            request.Description);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<object>.Success("Asset group updated successfully");
    }
}
