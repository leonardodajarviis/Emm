namespace Emm.Application.Features.AppOperationShift.Commands;

public class AddAssetsCommand : IRequest<Result>
{
    public Guid ShiftId { get; set; }
    public required AddAssetsData Data { get; init; }
}

public sealed record AddAssetsData
{
    public IReadOnlyCollection<Guid> AssetIds { get; init; } = [];
    public Guid? AssetBoxId { get; init; }
}
