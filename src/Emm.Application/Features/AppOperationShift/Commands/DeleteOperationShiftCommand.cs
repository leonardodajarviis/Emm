using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record DeleteOperationShiftCommand(
    Guid Id
) : IRequest<Result<object>>;
