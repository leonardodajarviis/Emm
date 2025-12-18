using Emm.Application.Common;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record UpdateAssetBoxCommand(
    Guid OperationShiftId,
    Guid AssetBoxId,
    string BoxName,
    BoxRole Role,
    int DisplayOrder,
    string? Description) : IRequest<Result<object>>;
