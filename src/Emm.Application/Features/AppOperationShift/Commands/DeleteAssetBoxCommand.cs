using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record DeleteAssetBoxCommand(
    Guid OperationShiftId,
    Guid AssetBoxId) : IRequest<Result<object>>;
