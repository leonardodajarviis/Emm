using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Entities.Operations.BusinessRules;
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
    public IReadOnlyCollection<OperationShiftReadingSnapshot> ReadingSnapshots => _readingSnapshots;

    public OperationShift(
        string code,
        string name,
        Guid primaryUserId,
        Guid organizationUnitId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime,
        string? description = null,
        string? notes = null)
    {
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
    }

    public void StartShift(DateTime actualStartTime, string? notes = null)
    {
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
            OperationShiftRules.EndTimeAfterStartTime,
            "Thời gian kết thúc phải sau thời gian bắt đầu");

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
        // Validate asset box exists if specified
        DomainGuard.AgainstNotFound(
            assetBoxId.HasValue && !_assetBoxes.Any(b => b.Id == assetBoxId.Value),
            $"Không tìm thấy nhóm tài sản vận hành với ID {assetBoxId}"
        );

        var asset = new OperationShiftAsset(
            Id, assetId, assetCode, assetName, isPrimary, assetBoxId);

        _assets.Add(asset);

        // Raise domain event
        Raise(new OperationShiftAssetAddedEvent(Id, assetId, assetCode, isPrimary));
    }

    public void Update(
        string name,
        string? description,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime)
    {
        // Prevent schedule changes if shift is in progress
        DomainGuard.AgainstBusinessRule(
            Status == OperationShiftStatus.InProgress &&
            (scheduledStartTime != ScheduledStartTime || scheduledEndTime != ScheduledEndTime),
            OperationShiftRules.CannotChangeScheduleInProgress,
            "Không thể thay đổi lịch trình của ca đang tiến hành");

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

    public void AssignAssetToBox(Guid assetId, Guid assetBoxId)
    {
        var asset = DomainGuard.AgainstNotFound(
            () => _assets.FirstOrDefault(a => a.AssetId == assetId),
            $"Không tìm thấy tài sản với ID {assetId} trong ca vận hành.");


        var box = DomainGuard.AgainstNotFound(
            () => _assetBoxes.FirstOrDefault(b => b.Id == assetBoxId),
            $"Không tìm thấy nhóm tài sản vận hành với ID {assetBoxId}");

        asset.AssignToGroup(assetBoxId);
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
