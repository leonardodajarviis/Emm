using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record AssignAssetToBoxCommand(
    Guid OperationShiftId,
    Guid AssetId,
    Guid? AssetBoxId) : IRequest<Result<object>>;
