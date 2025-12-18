using Emm.Application.Common;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record CreateAssetBoxCommand(
    Guid OperationShiftId,
    string BoxName,
    BoxRole Role,
    int DisplayOrder,
    string? Description,
    IReadOnlyCollection<Guid>? AssetIds) : IRequest<Result<object>>;
