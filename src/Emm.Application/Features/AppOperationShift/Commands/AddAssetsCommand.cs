using Emm.Application.Common;

namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddAssetsCommand : IRequest<Result<object>>
{
    public Guid ShiftId { get; set; }
    public IReadOnlyCollection<Guid> AssetIds { get; set; } = [];
    public Guid? AssetBoxId { get; set; }
}
