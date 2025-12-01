using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record CreateOperationShiftCommand(
    string Name,
    string? Notes,
    IReadOnlyCollection<long> AssetIds
) : IRequest<Result<object>>;
