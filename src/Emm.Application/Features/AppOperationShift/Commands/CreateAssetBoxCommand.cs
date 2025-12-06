using Emm.Application.Common;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record CreateAssetBoxCommand(
    long OperationShiftId,
    string BoxName,
    BoxRole Role,
    int DisplayOrder,
    string? Description,
    IReadOnlyCollection<long>? AssetIds) : IRequest<Result<object>>;
