namespace Emm.Application.Features.AppOperationShift.Commands;

/// <summary>
/// Command to resume a paused operation shift
/// </summary>
public record ResumeShiftCommand(Guid ShiftId, string? Notes = null) : IRequest<Result<object>>;
