using Emm.Application.Features.AppOperationShift.Commands;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Builder;

public class UpdateShiftLogContext
{
    public ShiftLog ShiftLog { get; init; } = null!;
    public IReadOnlyDictionary<Guid, OperationShiftAsset> AssetDict { get; init; } = null!;
    public UpdateShiftLogData Data { get; init; } = null!;
    public IDateTimeProvider Clock = null!;
}

public interface IUpdateShiftLogBuilderHandler
{
    Task<Result> Handle(UpdateShiftLogContext context, CancellationToken cancellationToken);
}
