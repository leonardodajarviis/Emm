namespace Emm.Application.Features.AppOperationShift.Commands;

public class RemoveShiftLogCommand : IRequest<Result<object>>
{
    public Guid OperationShiftId { get; set; }
    public Guid ShiftLogId { get; set; }
}
