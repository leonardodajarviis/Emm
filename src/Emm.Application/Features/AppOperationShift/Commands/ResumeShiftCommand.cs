namespace Emm.Application.Features.AppOperationShift.Commands;

/// <summary>
/// Command to resume a paused operation shift
/// </summary>
public record ResumeShiftCommand(long ShiftId, string? Notes = null) : IRequest<Result<object>>;
