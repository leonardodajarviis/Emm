using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CancelShiftCommand : IRequest<Result<object>>
{
    public long ShiftId { get; set; }
    public string Reason { get; set; } = null!;
}
