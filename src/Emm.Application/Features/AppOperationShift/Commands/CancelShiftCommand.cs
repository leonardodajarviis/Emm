namespace Emm.Application.Features.AppOperationShift.Commands;

public class CancelShiftCommand : IRequest<Result>
{
    public Guid ShiftId { get; set; }
    public string Reason { get; set; } = null!;
}
