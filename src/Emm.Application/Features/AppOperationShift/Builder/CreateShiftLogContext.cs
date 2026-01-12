using Emm.Application.Features.AppOperationShift.Commands;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Builder;
public class CreateShiftLogContext
{
    public ShiftLog ShiftLog { get; init; } = null!;
    public IReadOnlyDictionary<Guid, OperationShiftAsset> AssetDict { get; init; } = null!;
    public CreateShiftLogData Data { get; init; } = null!;
    public IDateTimeProvider Clock = null!;
}

public interface ICreateShiftLogBuilderHandler
{
    Task<Result> Handle(CreateShiftLogContext context, CancellationToken cancellationToken);
}
