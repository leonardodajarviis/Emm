using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddAssetsCommand : IRequest<Result<object>>
{
    public long ShiftId { get; set; }
    public IReadOnlyCollection<long> AssetIds { get; set; } = [];
}
