using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an operation shift is completed
/// </summary>
public sealed record OperationShiftCompletedEvent : IImmediateDomainEvent
{
    public long ShiftId { get; init; }
    public DateTime ActualEndTime { get; init; }
    public long PrimaryEmployeeId { get; init; }
    public string? Notes { get; init; }
    public DateTime OccurredOn { get; init; }

    public IReadOnlyCollection<long> AssetIds { get; init; } = [];

    public OperationShiftCompletedEvent(
        long shiftId,
        DateTime actualEndTime,
        long primaryEmployeeId,
        IReadOnlyCollection<long> assetIds,
        string? notes = null)
    {
        ShiftId = shiftId;
        ActualEndTime = actualEndTime;
        PrimaryEmployeeId = primaryEmployeeId;
        Notes = notes;
        OccurredOn = DateTime.UtcNow;
        AssetIds = assetIds;
    }
}
