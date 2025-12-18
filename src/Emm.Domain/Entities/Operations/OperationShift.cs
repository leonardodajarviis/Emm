using Emm.Domain.Abstractions;
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
    public OperationShiftStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    // Collections - using backing field pattern for EF Core
    private readonly List<OperationShiftAsset> _assets;
    public IReadOnlyCollection<OperationShiftAsset> Assets => _assets;

    private readonly List<OperationShiftAssetBox> _assetBoxes;
    public IReadOnlyCollection<OperationShiftAssetBox> AssetBoxes => _assetBoxes;

    public OperationShift(
        string code,
        string name,
        Guid primaryUserId,
        Guid organizationUnitId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime,
        string? notes = null)
    {
        // Validation
        ValidateCode(code);
        ValidateName(name);
        ValidateScheduleTimes(scheduledStartTime, scheduledEndTime);

        _assets = [];
        _assetBoxes = [];

        Code = code;
        Name = name;
        Notes = notes;
        PrimaryUserId = primaryUserId;
        OrganizationUnitId = organizationUnitId;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
        Status = OperationShiftStatus.Scheduled;
    }

    public OperationShift(
        string code,
        string name,
        Guid primaryUserId,
        Guid organizationUnitId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime,
        IEnumerable<OperationShiftAssetSpec> assets,
        string? description = null)
    {
        // Validation
        ValidateCode(code);
        ValidateName(name);
        ValidateScheduleTimes(scheduledStartTime, scheduledEndTime);

        _assets = [];
        _assetBoxes = [];

        Code = code;
        Name = name;
        PrimaryUserId = primaryUserId;
        OrganizationUnitId = organizationUnitId;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
        Description = description;
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

        DomainGuard.AgainstBusinessRule(
            actualStartTime > DateTime.UtcNow.AddMinutes(5),
            "ShiftCannotStartInFuture",
            "Cannot start shift with future time");

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

    public void RemoveAsset(Guid assetId)
    {
        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.Scheduled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot remove assets from shift in {Status} status. Shift must be in Scheduled status.");

        var asset = _assets.FirstOrDefault(a => a.AssetId == assetId);
        DomainGuard.AgainstNotFound(asset, "OperationShiftAsset", assetId);

        _assets.Remove(asset!);
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

        ValidateName(name);
        ValidateScheduleTimes(scheduledStartTime, scheduledEndTime);

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

    public void ChangePrimaryUser(Guid newPrimaryUserId)
    {
        DomainGuard.AgainstInvalidForeignKey(newPrimaryUserId, nameof(newPrimaryUserId));

        DomainGuard.AgainstInvalidState(
            Status == OperationShiftStatus.Completed || Status == OperationShiftStatus.Cancelled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot change primary User for shift in {Status} status");

        PrimaryUserId = newPrimaryUserId;
    }

    public void PauseShift(string reason)
    {
        DomainGuard.AgainstNullOrEmpty(reason, "PauseReason");

        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.InProgress,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot pause shift in {Status} status. Shift must be in InProgress status.");

        Status = OperationShiftStatus.Paused;
        Notes = string.IsNullOrEmpty(Notes)
            ? $"Paused: {reason}"
            : $"{Notes}\nPaused: {reason}";

        // Raise domain event
        Raise(new OperationShiftPausedEvent(Id, reason));
    }

    public void ResumeShift(string? notes = null)
    {
        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.Paused,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot resume shift in {Status} status. Shift must be in Paused status.");

        Status = OperationShiftStatus.InProgress;
        if (!string.IsNullOrEmpty(notes))
        {
            Notes = string.IsNullOrEmpty(Notes)
                ? $"Resumed: {notes}"
                : $"{Notes}\nResumed: {notes}";
        }

        // Raise domain event
        Raise(new OperationShiftResumedEvent(Id, notes));
    }

    public void MarkAsOverdue(string reason)
    {
        DomainGuard.AgainstNullOrEmpty(reason, "OverdueReason");

        DomainGuard.AgainstInvalidState(
            Status == OperationShiftStatus.Completed || Status == OperationShiftStatus.Cancelled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot mark shift as overdue in {Status} status");

        DomainGuard.AgainstBusinessRule(
            DateTime.UtcNow <= ScheduledEndTime,
            "ShiftNotYetOverdue",
            "Cannot mark shift as overdue before scheduled end time");

        Status = OperationShiftStatus.Overdue;
        Notes = string.IsNullOrEmpty(Notes)
            ? $"Overdue: {reason}"
            : $"{Notes}\nOverdue: {reason}";

        // Raise domain event
        Raise(new OperationShiftOverdueEvent(Id, reason));
    }

    public void Reschedule(
        DateTime newScheduledStartTime,
        DateTime newScheduledEndTime,
        string reason)
    {
        DomainGuard.AgainstNullOrEmpty(reason, "RescheduleReason");
        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.Scheduled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot reschedule shift in {Status} status. Shift must be in Scheduled status.");

        ValidateScheduleTimes(newScheduledStartTime, newScheduledEndTime);

        var oldStartTime = ScheduledStartTime;
        var oldEndTime = ScheduledEndTime;

        ScheduledStartTime = newScheduledStartTime;
        ScheduledEndTime = newScheduledEndTime;
        Notes = string.IsNullOrEmpty(Notes)
            ? $"Rescheduled: {reason}"
            : $"{Notes}\nRescheduled: {reason}";

        // Raise domain event
        Raise(new OperationShiftRescheduledEvent(
            Id, oldStartTime, oldEndTime, newScheduledStartTime, newScheduledEndTime, reason));
    }

    public void Reactivate(
        DateTime newScheduledStartTime,
        DateTime newScheduledEndTime,
        string reason)
    {
        DomainGuard.AgainstNullOrEmpty(reason, "ReactivateReason");

        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.Cancelled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot reactivate shift in {Status} status. Shift must be in Cancelled status.");

        ValidateScheduleTimes(newScheduledStartTime, newScheduledEndTime);

        Status = OperationShiftStatus.Scheduled;
        ScheduledStartTime = newScheduledStartTime;
        ScheduledEndTime = newScheduledEndTime;
        ActualStartTime = null;
        ActualEndTime = null;
        Notes = string.IsNullOrEmpty(Notes)
            ? $"Reactivated: {reason}"
            : $"{Notes}\nReactivated: {reason}";

        // Raise domain event
        Raise(new OperationShiftReactivatedEvent(
            Id, newScheduledStartTime, newScheduledEndTime, reason));
    }

    #region Asset box Management

    public void CreateAssetBox(
        string boxName,
        BoxRole role = BoxRole.Secondary,
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

    public IEnumerable<OperationShiftAsset> GetAssetsBybox(Guid assetBoxId)
    {
        return _assets.Where(a => a.AssetBoxId == assetBoxId);
    }

    public IEnumerable<OperationShiftAsset> GetUnboxedAssets()
    {
        return _assets.Where(a => !a.AssetBoxId.HasValue);
    }

    #endregion

    #region Validation Methods

    private static void ValidateCode(string code)
    {
        DomainGuard.AgainstNullOrEmpty(code, nameof(Code));
        DomainGuard.AgainstTooLong(code, 50, nameof(Code));
    }

    private static void ValidateName(string name)
    {
        DomainGuard.AgainstNullOrEmpty(name, nameof(Name));
        DomainGuard.AgainstTooLong(name, 200, nameof(Name));
    }

    private static void ValidateScheduleTimes(DateTime startTime, DateTime endTime)
    {
        DomainGuard.AgainstBusinessRule(
            endTime <= startTime,
            "ShiftEndTimeMustBeAfterStartTime",
            "Scheduled end time must be after start time");

        var duration = endTime - startTime;
        DomainGuard.AgainstBusinessRule(
            duration.TotalHours > 24,
            "ShiftDurationExceeds24Hours",
            "Shift duration cannot exceed 24 hours");
    }
    #endregion

    private OperationShift()
    {
        _assets = [];
        _assetBoxes = [];
    } // EF Core constructor
}

public enum OperationShiftStatus
{
    Scheduled = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3,
    Overdue = 4,
    Paused = 5
}
