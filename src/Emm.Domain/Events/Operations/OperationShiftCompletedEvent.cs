using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is completed
/// </summary>
public sealed record OperationShiftCompletedEvent : IImmediateDomainEvent
{
    public Guid ShiftId { get; init; }
    public DateTime ActualEndTime { get; init; }
    public Guid PrimaryUserId { get; init; }
    public string? Notes { get; init; }
    public DateTime OccurredOn { get; init; }

    public IReadOnlyCollection<Guid> AssetIds { get; init; } = [];

    public OperationShiftCompletedEvent(
        Guid shiftId,
        DateTime actualEndTime,
        Guid primaryUserId,
        IReadOnlyCollection<Guid> assetIds,
        string? notes = null)
    {
        ShiftId = shiftId;
        ActualEndTime = actualEndTime;
        PrimaryUserId = primaryUserId;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
        AssetIds = assetIds;
    }
}
