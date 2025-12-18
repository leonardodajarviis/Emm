using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record UpdateOperationShiftCommand(
    Guid Id,
    string Name,
    string? Description,
    Guid LocationId,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime,
    string? Notes
) : IRequest<Result<object>>;
