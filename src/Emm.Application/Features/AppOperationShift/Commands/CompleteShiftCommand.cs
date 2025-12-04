using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class CompleteShiftCommand : IRequest<Result<object>>
{
    public long ShiftId { get; set; }
}
