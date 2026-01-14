namespace Emm.Application.Features.AppOperationShift.Commands;

public record StartOperationShiftCommand(Guid ShiftId) : IRequest<Result>;
