namespace Emm.Application.Features.AppOperationShift.Commands;

public class CompleteShiftCommand : IRequest<Result<object>>
{
    public Guid ShiftId { get; set; }
}
