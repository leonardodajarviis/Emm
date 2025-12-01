namespace Emm.Application.Features.AppOperationShift.Commands;

/// <summary>
/// Command to mark an operation shift as overdue
/// </summary>
public record MarkOverdueShiftCommand(long ShiftId, string Reason) : IRequest<Result<object>>;
