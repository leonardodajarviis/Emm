using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record AssignAssetToGroupCommand(
    long OperationShiftId,
    long AssetId,
    long? AssetGroupId) : IRequest<Result<object>>;
