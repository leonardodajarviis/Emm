namespace Emm.Application.Features.AppOperationShift.Commands;

public record StartOperationShiftCommand(long ShiftId) : IRequest<Result<object>>;