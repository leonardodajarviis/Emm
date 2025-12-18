using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Operations;

/// <summary>
/// Domain event raised when an asset is added to an operation shift
/// </summary>
public sealed record OperationShiftAssetAddedEvent : IDomainEvent
{
    public Guid ShiftId { get; init; }
    public Guid AssetId { get; init; }
    public string AssetCode { get; init; }
    public bool IsPrimary { get; init; }
    public DateTime OccurredOn { get; init; }

    public OperationShiftAssetAddedEvent(
        Guid shiftId,
        Guid assetId,
        string assetCode,
        bool isPrimary)
    {
        ShiftId = shiftId;
        AssetId = assetId;
        AssetCode = assetCode;
        IsPrimary = isPrimary;
        OccurredOn = DateTime.UtcNow;
    }
}
