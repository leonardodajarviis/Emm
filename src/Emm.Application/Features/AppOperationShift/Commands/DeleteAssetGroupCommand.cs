using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record DeleteAssetGroupCommand(
    long OperationShiftId,
    long AssetGroupId) : IRequest<Result<object>>;
