namespace Emm.Application.Features.AppOperationShift.Commands;

/// <summary>
/// Command to reschedule an operation shift that is in Scheduled status
/// </summary>
public class RescheduleShiftCommand : IRequest<Result<object>>
{
    public long ShiftId { get; set; }
    public DateTime NewScheduledStartTime { get; set; }
    public DateTime NewScheduledEndTime { get; set; }
    public string Reason { get; set; } = null!;
}
