using Emm.Application.Common;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record UpdateAssetBoxCommand(
    long OperationShiftId,
    long AssetBoxId,
    string BoxName,
    BoxRole Role,
    int DisplayOrder,
    string? Description) : IRequest<Result<object>>;
