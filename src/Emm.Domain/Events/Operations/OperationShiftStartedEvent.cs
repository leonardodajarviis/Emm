using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is started
/// </summary>
public sealed record OperationShiftStartedEvent : IImmediateDomainEvent
{
    public Guid ShiftId { get; init; }
    public DateTime ActualStartTime { get; init; }
    public Guid PrimaryUserId { get; init; }
    public IReadOnlyCollection<Guid> AssetIds { get; init; } = [];
    public DateTime OccurredOn { get; init; }

    public OperationShiftStartedEvent(
        Guid shiftId,
        DateTime actualStartTime,
        Guid primaryUserId,
        IReadOnlyCollection<Guid> assetIds)
    {
        ShiftId = shiftId;
        ActualStartTime = actualStartTime;
        PrimaryUserId = primaryUserId;
        OccurredOn = DateTime.UtcNow;
        AssetIds = assetIds;
    }
}
