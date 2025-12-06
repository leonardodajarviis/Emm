using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record DeleteAssetBoxCommand(
    long OperationShiftId,
    long AssetBoxId) : IRequest<Result<object>>;
