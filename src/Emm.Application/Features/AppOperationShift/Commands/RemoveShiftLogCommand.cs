namespace Emm.Application.Features.AppOperationShift.Commands;

public class RemoveShiftLogCommand : IRequest<Result<object>>
{
    public long OperationShiftId { get; set; }
    public long ShiftLogId { get; set; }
}
