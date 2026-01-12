using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations.CreationData;
using Emm.Domain.Events.Operations;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Ca vận hành thiết bị - Aggregate Root
/// </summary>
public class OperationShift : AggregateRoot, IAuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid OrganizationUnitId { get; private set; }
    public Guid PrimaryUserId { get; private set; }
    public bool IsCheckpointLogEnabled { get; private set; }
    public DateTime ScheduledStartTime { get; private set; }
    public DateTime ScheduledEndTime { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public Guid? CurrentShiftLogId { get; private set; }
    public OperationShiftStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    // Collections - using backing field pattern for EF Core
    private readonly List<OperationShiftAsset> _assets;
    public IReadOnlyCollection<OperationShiftAsset> Assets => _assets;

    private readonly List<OperationShiftAssetBox> _assetBoxes;
    public IReadOnlyCollection<OperationShiftAssetBox> AssetBoxes => _assetBoxes;

    private readonly List<OperationShiftReadingSnapshot> _readingSnapshots;
    public IReadOnlyCollection<OperationShiftReadingSnapshot> GetReadingSnapshots() => _readingSnapshots;

    public OperationShift(
        string code,
        string name,
        Guid primaryUserId,
        Guid organizationUnitId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime,
        IEnumerable<AssignAssetData> assets,
        string? description = null,
        string? notes = null)
    {
        DomainGuard.AgainstNullOrEmpty(code, nameof(Code));
        DomainGuard.AgainstTooLong(code, 50, nameof(Code));

        DomainGuard.AgainstNullOrEmpty(name, nameof(Name));
        DomainGuard.AgainstTooLong(name, 200, nameof(Name));

        if (assets == null || !assets.Any())
        {
            throw new DomainException("At least one asset must be assigned to the operation shift");
        }

        _assets = [];
        _assetBoxes = [];
        _readingSnapshots = [];


        Code = code;
        Name = name;
        PrimaryUserId = primaryUserId;
        OrganizationUnitId = organizationUnitId;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
        Description = description;
        Notes = notes;
        Status = OperationShiftStatus.Scheduled;

        foreach (var asset in assets)
        {
            AddAsset(
                assetId: asset.AssetId,
                assetCode: asset.AssetCode,
                assetName: asset.AssetName,
                isPrimary: asset.IsPrimary);
        }
    }

    public void StartShift(DateTime actualStartTime, string? notes = null)
    {
        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.Scheduled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot start shift in {Status} status. Shift must be in Scheduled status.");

        ActualStartTime = actualStartTime;
        Status = OperationShiftStatus.InProgress;
        Notes = notes;

        var assetIds = _assets.Select(a => a.AssetId).ToList();

        // Raise domain event
        Raise(new OperationShiftStartedEvent(Id, actualStartTime, PrimaryUserId, assetIds));
    }

    public void CompleteShift(DateTime actualEndTime, string? notes = null)
    {
        DomainGuard.AgainstBusinessRule(
            ActualStartTime.HasValue && actualEndTime < ActualStartTime.Value,
            "ShiftEndTimeBeforeStartTime",
            "Shift end time cannot be before start time");

        ActualEndTime = actualEndTime;
        Status = OperationShiftStatus.Completed;
        if (!string.IsNullOrEmpty(notes))
            Notes = notes;

        var assetIds = _assets.Select(a => a.AssetId).ToList();

        // Raise domain event
        Raise(new OperationShiftCompletedEvent(Id, actualEndTime, PrimaryUserId, assetIds, notes));
    }

    public void CancelShift(string reason)
    {
        DomainGuard.AgainstNullOrEmpty(reason, "CancelReason");

        DomainGuard.AgainstInvalidState(
            Status == OperationShiftStatus.Completed || Status == OperationShiftStatus.Cancelled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot cancel shift in {Status} status");

        Status = OperationShiftStatus.Cancelled;
        Notes = reason;

        // Raise domain event
        Raise(new OperationShiftCancelledEvent(Id, reason));
    }

    public void AddAsset(
        Guid assetId,
        string assetCode,
        string assetName,
        bool isPrimary = false,
        Guid? assetBoxId = null)
    {
        DomainGuard.AgainstInvalidForeignKey(assetId, nameof(assetId));
        DomainGuard.AgainstNullOrEmpty(assetCode, nameof(assetCode));
        DomainGuard.AgainstNullOrEmpty(assetName, nameof(assetName));

        // Check for duplicate asset
        DomainGuard.AgainstDuplicate(
            _assets.Any(a => a.AssetId == assetId),
            "OperationShiftAsset",
            nameof(assetId),
            assetId);

        // Validate primary asset rule - only one primary asset allowed
        DomainGuard.AgainstBusinessRule(
            isPrimary && _assets.Any(a => a.IsPrimary),
            "OnlyOnePrimaryAssetAllowed",
            "Only one primary asset is allowed per shift");

        // Validate asset box exists if specified
        if (assetBoxId.HasValue)
        {
            var box = _assetBoxes.FirstOrDefault(g => g.Id == assetBoxId.Value);
            DomainGuard.AgainstNotFound(
                box,
                "OperationShiftAssetBox",
                assetBoxId.Value);
        }

        var asset = new OperationShiftAsset(
            Id, assetId, assetCode, assetName, isPrimary, assetBoxId);

        _assets.Add(asset);

        // Raise domain event
        Raise(new OperationShiftAssetAddedEvent(Id, assetId, assetCode, isPrimary));
    }

    public void Update(
        string name,
        string? description,
        Guid locationId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime)
    {
        // Prevent updates to completed or cancelled shifts
        DomainGuard.AgainstInvalidState(
            Status == OperationShiftStatus.Completed || Status == OperationShiftStatus.Cancelled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot update shift in {Status} status");

        // Prevent schedule changes if shift is in progress
        DomainGuard.AgainstBusinessRule(
            Status == OperationShiftStatus.InProgress &&
            (scheduledStartTime != ScheduledStartTime || scheduledEndTime != ScheduledEndTime),
            "CannotChangeScheduleInProgress",
            "Cannot change schedule times for shift in progress");

        DomainGuard.AgainstInvalidForeignKey(locationId, nameof(locationId));

        Name = name;
        Description = description;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }

    public void SetCurrentShiftLog(Guid shiftLogId)
    {
        CurrentShiftLogId = shiftLogId;
    }

    public void AddReadingSnapshot(Guid assetId, Guid parameterId, decimal value)
    {
        var snapshot = new OperationShiftReadingSnapshot(
            Id, assetId, parameterId, value);

        _readingSnapshots.Add(snapshot);
    }

    #region Asset box Management

    public void CreateAssetBox(
        string boxName,
        BoxRole role = BoxRole.Secondary,
        IEnumerable<Guid>? assetIds = null,
        int displayOrder = 0,
        string? description = null)
    {
        // Validate unique box name within this shift
        DomainGuard.AgainstDuplicate(
            _assetBoxes.Any(b => b.BoxName.Equals(boxName, StringComparison.OrdinalIgnoreCase)),
            "OperationShiftAssetBox",
            nameof(boxName),
            boxName);

        var box = new OperationShiftAssetBox(
            Id, boxName, role, displayOrder, description);

        _assetBoxes.Add(box);

        if (assetIds != null)
        {
            foreach (var assetId in assetIds)
            {
                AssignAssetToBox(assetId, box.Id);
            }
        }
    }

    public void UpdateAssetBox(
        Guid assetBoxId,
        string boxName,
        BoxRole role,
        int displayOrder,
        string? description = null)
    {
        var box = _assetBoxes.FirstOrDefault(g => g.Id == assetBoxId);
        DomainGuard.AgainstNotFound(box, "OperationShiftAssetBox", assetBoxId);

        // Validate unique box name within this shift (excluding current box)
        DomainGuard.AgainstDuplicate(
            _assetBoxes.Any(b => b.Id != assetBoxId && b.BoxName.Equals(boxName, StringComparison.OrdinalIgnoreCase)),
            "OperationShiftAssetBox",
            nameof(boxName),
            boxName);

        box!.Update(boxName, role, displayOrder, description);
    }

    public void RemoveAssetBox(Guid assetBoxId)
    {
        var box = _assetBoxes.FirstOrDefault(g => g.Id == assetBoxId);
        DomainGuard.AgainstNotFound(box, "OperationShiftAssetBox", assetBoxId);

        // Check if any assets are still assigned to this box
        DomainGuard.AgainstBusinessRule(
            _assets.Any(a => a.AssetBoxId == assetBoxId),
            "CannotRemoveboxWithAssets",
            $"Cannot remove asset box {assetBoxId} because it still has assets assigned to it");

        _assetBoxes.Remove(box!);
    }

    public void AssignAssetToBox(Guid assetId, Guid? assetBoxId)
    {
        var asset = _assets.FirstOrDefault(a => a.AssetId == assetId);
        DomainGuard.AgainstNotFound(asset, "OperationShiftAsset", assetId);

        // Validate asset box exists if specified
        if (assetBoxId.HasValue)
        {
            var box = _assetBoxes.FirstOrDefault(g => g.Id == assetBoxId.Value);
            DomainGuard.AgainstNotFound(box, "OperationShiftAssetBox", assetBoxId.Value);
        }

        asset!.AssignToGroup(assetBoxId);
    }

    #endregion

    private OperationShift()
    {
        _readingSnapshots = [];
        _assets = [];
        _assetBoxes = [];
        Status = OperationShiftStatus.Scheduled;
    }
}
