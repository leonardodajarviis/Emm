using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is started
/// </summary>
public sealed record OperationShiftStartedEvent : IImmediateDomainEvent
{
    public long ShiftId { get; init; }
    public DateTime ActualStartTime { get; init; }
    public long PrimaryEmployeeId { get; init; }
    public IReadOnlyCollection<long> AssetIds { get; init; } = [];
    public DateTime OccurredOn { get; init; }

    public OperationShiftStartedEvent(
        long shiftId,
        DateTime actualStartTime,
        long primaryEmployeeId,
        IReadOnlyCollection<long> assetIds)
    {
        ShiftId = shiftId;
        ActualStartTime = actualStartTime;
        PrimaryEmployeeId = primaryEmployeeId;
        OccurredOn = DateTime.UtcNow;
        AssetIds = assetIds;
    }
}
