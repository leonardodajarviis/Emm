namespace Emm.Application.Features.AppOperationShift.Commands;

/// <summary>
/// Command to pause an operation shift that is currently in progress
/// </summary>
public record PauseShiftCommand(Guid ShiftId, string Reason) : IRequest<Result<object>>;
