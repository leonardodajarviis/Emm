using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record AssignAssetToBoxCommand(
    long OperationShiftId,
    long AssetId,
    long? AssetBoxId) : IRequest<Result<object>>;
