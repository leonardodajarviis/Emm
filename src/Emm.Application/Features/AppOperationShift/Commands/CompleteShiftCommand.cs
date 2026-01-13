namespace Emm.Application.Features.AppOperationShift.Commands;

public class CompleteShiftCommand : IRequest<Result>
{
    public Guid ShiftId { get; set; }
}
