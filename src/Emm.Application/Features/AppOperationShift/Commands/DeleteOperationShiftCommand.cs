using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record DeleteOperationShiftCommand(
    long Id
) : IRequest<Result<object>>;
