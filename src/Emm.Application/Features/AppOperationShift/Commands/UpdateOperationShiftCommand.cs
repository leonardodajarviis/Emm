using LazyNet.Symphony.Interfaces;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record UpdateOperationShiftCommand(
    long Id,
    string Name,
    string? Description,
    long LocationId,
    DateTime ScheduledStartTime,
    DateTime ScheduledEndTime,
    string? Notes
) : IRequest<Result<object>>;
