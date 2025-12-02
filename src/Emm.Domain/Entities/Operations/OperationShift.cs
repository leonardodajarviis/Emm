using Emm.Domain.Abstractions;
using Emm.Domain.Events.Operations;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Ca vận hành thiết bị - Aggregate Root
/// </summary>
public class OperationShift : AggregateRoot, IAuditableEntity
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public long OrganizationUnitId { get; private set; }
    public long PrimaryUserId { get; private set; }
    public bool IsCheckpointLogEnabled { get; private set; }
    public DateTime ScheduledStartTime { get; private set; }
    public DateTime ScheduledEndTime { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public OperationShiftStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Collections - using backing field pattern for EF Core
    private readonly List<OperationShiftAsset> _assets;
    public IReadOnlyCollection<OperationShiftAsset> Assets => _assets;

    public OperationShift(
        string code,
        string name,
        long primaryEmployeeId,
        long organizationUnitId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime,
        string? notes = null)
    {
        // Validation
        ValidateCode(code);
        ValidateName(name);
        ValidateScheduleTimes(scheduledStartTime, scheduledEndTime);

        _assets = [];

        Code = code;
        Name = name;
        Notes = notes;
        PrimaryUserId = primaryEmployeeId;
        OrganizationUnitId = organizationUnitId;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
        Status = OperationShiftStatus.Scheduled;
    }

    public OperationShift(
        string code,
        string name,
        long primaryEmployeeId,
        long organizationUnitId,
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

        Code = code;
        Name = name;
        PrimaryUserId = primaryEmployeeId;
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
        DomainGuard.AgainstInvalidState(
            Status != OperationShiftStatus.InProgress,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot complete shift in {Status} status. Shift must be in InProgress status.");

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
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event
        Raise(new OperationShiftCancelledEvent(Id, reason));
    }

    public void AddAsset(
        long assetId,
        string assetCode,
        string assetName,
        bool isPrimary = false)
    {
        DomainGuard.AgainstNegativeOrZero(assetId, nameof(assetId));
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

        var asset = new OperationShiftAsset(
            Id, assetId, assetCode, assetName, isPrimary);

        _assets.Add(asset);

        // Raise domain event
        Raise(new OperationShiftAssetAddedEvent(Id, assetId, assetCode, isPrimary));
    }

    public void RemoveAsset(long assetId)
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
        long locationId,
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

        DomainGuard.AgainstNegativeOrZero(locationId, nameof(locationId));

        Name = name;
        Description = description;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePrimaryEmployee(long newPrimaryEmployeeId)
    {
        DomainGuard.AgainstNegativeOrZero(newPrimaryEmployeeId, nameof(newPrimaryEmployeeId));

        DomainGuard.AgainstInvalidState(
            Status == OperationShiftStatus.Completed || Status == OperationShiftStatus.Cancelled,
            nameof(OperationShift),
            Status.ToString(),
            $"Cannot change primary employee for shift in {Status} status");

        PrimaryUserId = newPrimaryEmployeeId;
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
        UpdatedAt = DateTime.UtcNow;

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
        UpdatedAt = DateTime.UtcNow;

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
        UpdatedAt = DateTime.UtcNow;

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
        UpdatedAt = DateTime.UtcNow;

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
        UpdatedAt = DateTime.UtcNow;

        // Raise domain event
        Raise(new OperationShiftReactivatedEvent(
            Id, newScheduledStartTime, newScheduledEndTime, reason));
    }

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
