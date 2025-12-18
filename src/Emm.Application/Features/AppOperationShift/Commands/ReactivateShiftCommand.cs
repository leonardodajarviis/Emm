namespace Emm.Application.Features.AppOperationShift.Commands;

/// <summary>
/// Command to reactivate a cancelled operation shift
/// </summary>
public class ReactivateShiftCommand : IRequest<Result<object>>
{
    public Guid ShiftId { get; set; }
    public DateTime NewScheduledStartTime { get; set; }
    public DateTime NewScheduledEndTime { get; set; }
    public string Reason { get; set; } = null!;
}
