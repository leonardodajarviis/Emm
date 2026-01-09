using Emm.Domain.Abstractions;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Operations;

public class ShiftLog : AggregateRoot, IAuditableEntity
{
    public int LogOrder { get; private set; }
    public Guid OperationShiftId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>
    /// Asset ID cho trường hợp ghi log cho 1 asset cụ thể
    /// Null nếu là shift-level log hoặc ghi log theo group
    /// </summary>
    public Guid? AssetId { get; private set; }

    /// <summary>
    /// Group ID cho trường hợp ghi log cho nhiều assets theo group
    /// Null nếu là shift-level log hoặc ghi log cho 1 asset cụ thể
    /// </summary>
    public Guid? BoxId { get; private set; }

    private readonly List<ShiftLogParameterReading> _readings;
    public IReadOnlyCollection<ShiftLogParameterReading> Readings => _readings;

    private readonly List<ShiftLogCheckpoint> _checkpoints;
    public IReadOnlyCollection<ShiftLogCheckpoint> Checkpoints => _checkpoints;

    private readonly List<ShiftLogEvent> _events;
    public IReadOnlyCollection<ShiftLogEvent> Events => _events;

    private readonly List<ShiftLogItem> _items;
    public IReadOnlyCollection<ShiftLogItem> Items => _items;

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    public ShiftLog(
        Guid operationShiftId,
        string name,
        DateTime startTime,
        DateTime? endTime = null,
        Guid? assetId = null,
        Guid? boxId = null)
    {
        DomainGuard.AgainstInvalidForeignKey(operationShiftId, nameof(operationShiftId));

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Task name is required");

        // Validation: Không được set cả AssetId và GroupId cùng lúc
        if (assetId.HasValue && boxId.HasValue)
            throw new DomainException("Cannot set both AssetId and GroupId. Choose either single asset or group.");

        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];

        OperationShiftId = operationShiftId;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        AssetId = assetId;
        BoxId = boxId;
    }

    public void UpdateStartTime(DateTime startTime)
    {
        StartTime = startTime;
    }

    public void UpdateEndTime(DateTime endTime)
    {
        if (endTime < StartTime)
            throw new DomainException("End time cannot be before start time");

        EndTime = endTime;
    }

    public void UpdateEvent(
        Guid eventId,
        ShiftLogEventType eventType,
        DateTime startTime,
        DateTime? endTime = null)
    {
        var statusHistory = _events.FirstOrDefault(h => h.Id == eventId);
        if (statusHistory == null)
            throw new DomainException($"Event with ID {eventId} not found");

        statusHistory.Update(eventType, startTime, endTime);
    }

    #region Asset and Group Management

    /// <summary>
    /// Gán ShiftLog cho một asset cụ thể
    /// </summary>
    public void AssignToAsset(Guid assetId)
    {
        DomainGuard.AgainstInvalidForeignKey(assetId, nameof(assetId));

        if (BoxId.HasValue)
            throw new DomainException("Cannot assign to asset when already assigned to a group");

        AssetId = assetId;
    }

    /// <summary>
    /// Gán ShiftLog cho một group (nhiều assets)
    /// </summary>
    public void AssignToBox(Guid boxId)
    {
        DomainGuard.AgainstInvalidForeignKey(boxId, nameof(boxId));

        if (AssetId.HasValue)
            throw new DomainException("Cannot assign to group when already assigned to an asset");

        BoxId = boxId;
    }

    /// <summary>
    /// Bỏ gán asset/group (chuyển về shift-level log)
    /// </summary>
    public void UnassignAssetOrBox()
    {
        AssetId = null;
        BoxId = null;
    }

    /// <summary>
    /// Kiểm tra xem ShiftLog có gắn với asset/group không
    /// </summary>
    public bool IsAssigned()
    {
        return AssetId.HasValue || BoxId.HasValue;
    }

    /// <summary>
    /// Kiểm tra xem ShiftLog có gắn với một asset cụ thể không
    /// </summary>
    public bool IsAssignedToAsset()
    {
        return AssetId.HasValue;
    }

    /// <summary>
    /// Kiểm tra xem ShiftLog có gắn với group không
    /// </summary>
    public bool IsAssignedToBox()
    {
        return BoxId.HasValue;
    }

    #endregion

    public void LockReading(Guid readingId)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId);
        if (reading == null)
            throw new DomainException($"Reading with ID {readingId} not found");

        reading.Locked();
    }

    public void LockAllReadings()
    {
        foreach (var reading in _readings)
        {
            reading.Locked();
        }
    }

    public void AddReading(
        Guid assetId,
        string assetCode,
        string assetName,
        Guid parameterId,
        string parameterName,
        string parameterCode,
        Guid unitOfMeasureId,
        decimal value,
        Guid? shiftLogCheckPointLinkedId = null)
    {
        DomainGuard.AgainstInvalidForeignKey(assetId, nameof(assetId));
        DomainGuard.AgainstInvalidForeignKey(parameterId, nameof(parameterId));


        var reading = new ShiftLogParameterReading(
            Id, assetId, assetCode, assetName,
            parameterId, parameterName, parameterCode, unitOfMeasureId,
            value, shiftLogCheckPointLinkedId);

        _readings.Add(reading);
    }

    /// <summary>
    /// Xóa reading theo ID
    /// </summary>
    public void RemoveReading(Guid readingId)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId) ?? throw new DomainException($"Reading with ID {readingId} not found");

        _readings.Remove(reading);
    }

    public void UpdateReadingValue(Guid readingId, decimal newValue)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId) ?? throw new DomainException($"Reading with ID {readingId} not found");
        reading.UpdateValue(newValue);
    }

    public void AddCheckpoint(Guid linkedId, string name, Guid locationId, string locationName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Checkpoint name is required");

        var checkpoint = new ShiftLogCheckpoint(
            Id,
            linkedId,
            name,
            locationId,
            locationName
        );

        _checkpoints.Add(checkpoint);
    }

    public void MakeAttchedMaterialInCheckpoint(Guid checkpointId, Guid itemId, string itemCode, string itemName)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint == null)
            throw new DomainException($"Checkpoint with ID {checkpointId} not found");

        if (checkpoint.ItemId == itemId)
            return; // Already set

        checkpoint.MakeAttachedMaterial(itemId, itemCode, itemName);
    }

    public void UpdateLocationInCheckpoint(Guid checkpointId, Guid locationId, string locationName)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint == null)
            throw new DomainException($"Checkpoint with ID {checkpointId} not found");

        if (checkpoint.LocationId == locationId) return;

        checkpoint.UpdateLocation(locationId, locationName);
    }

    /// <summary>
    /// Xóa checkpoint theo ID
    /// </summary>
    public void RemoveCheckpoint(Guid checkpointId)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint == null)
            throw new DomainException($"Checkpoint with ID {checkpointId} not found");

        _checkpoints.Remove(checkpoint);
    }

    /// <summary>
    /// Ghi nhận sự kiện: nghỉ ca, sự cố, ghi chú quan trọng
    /// </summary>
    public void RecordEvent(
        ShiftLogEventType eventType,
        DateTime startTime,
        DateTime? endTime = null)
    {

        var statusHistory = new ShiftLogEvent(Id, eventType, startTime, endTime);

        _events.Add(statusHistory);
    }

    // Removed RecordEvents - use ShiftLogSyncService.RecordBulkEvents instead

    /// <summary>
    /// Kết thúc sự kiện đang mở (nghỉ ca, sự cố)
    /// </summary>
    public void EndEvent(Guid eventId, DateTime endTime)
    {
        var statusHistory = _events.FirstOrDefault(h => h.Id == eventId);
        if (statusHistory == null)
            throw new DomainException($"Event with ID {eventId} not found");

        statusHistory.EndEvent(endTime);
    }

    /// <summary>
    /// Xóa event theo ID
    /// </summary>
    public void RemoveEvent(Guid eventId)
    {
        var statusHistory = _events.FirstOrDefault(h => h.Id == eventId);
        if (statusHistory == null)
            throw new DomainException($"Event with ID {eventId} not found");

        _events.Remove(statusHistory);
    }

    /// <summary>
    /// Thêm item vào shift log
    /// </summary>
    public void AddItem(
        Guid itemId,
        Guid? warehouseIssueSlipId,
        string itemCode,
        string itemName,
        decimal quantity,
        Guid? assetId = null,
        string? assetCode = null,
        string? assetName = null,
        Guid? unitOfMeasureId = null,
        string? unitOfMeasureName = null)
    {
        DomainGuard.AgainstInvalidForeignKey(itemId, nameof(itemId));

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Item name is required");

        var item = new ShiftLogItem(
            Id, warehouseIssueSlipId, itemId, itemName, itemCode, quantity,
            assetId, assetCode, assetName,
            unitOfMeasureId, unitOfMeasureName);

        _items.Add(item);
    }


    /// <summary>
    /// Xóa item theo ID
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException($"Item with ID {itemId} not found");

        _items.Remove(item);
    }

    public void UpdateItemQuantity(Guid itemId, decimal newQuantity)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException($"Item with ID {itemId} not found");

        item.UpdateQuantity(newQuantity);
    }

    private ShiftLog()
    {
        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];
    } // EF Core constructor
}
