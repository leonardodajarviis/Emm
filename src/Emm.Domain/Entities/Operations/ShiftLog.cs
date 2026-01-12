using Emm.Domain.Abstractions;
using Emm.Domain.Events.ShiftLogs;
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
    /// Box ID cho trường hợp ghi log cho nhiều assets theo group
    /// Null nếu là shift-level log hoặc ghi log cho 1 asset cụ thể
    /// </summary>
    public Guid? BoxId { get; private set; }

    public Guid? LocationId { get; private set; }
    public string? LocationName { get; private set; }

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
    private readonly List<ShiftLogParameterReadingEventData> _readingEvents = [];
    public bool IsLooked { get; private set; }

    public ShiftLog(
        int logOrder,
        Guid operationShiftId,
        string name,
        DateTime startTime,
        DateTime? endTime,
        Guid? assetId,
        Guid? boxId,
        Guid? locationId,
        string? locationName)
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

        LogOrder = logOrder;
        OperationShiftId = operationShiftId;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        AssetId = assetId;
        BoxId = boxId;
        LocationId = locationId;
        LocationName = locationName;
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
        ParameterType parameterType,
        Guid unitOfMeasureId,
        decimal value,
        DateTime readingTime,
        int groupNumber,
        Guid? shiftLogCheckPointLinkedId = null)
    {
        DomainGuard.AgainstInvalidForeignKey(assetId, nameof(assetId));
        DomainGuard.AgainstInvalidForeignKey(parameterId, nameof(parameterId));


        var reading = new ShiftLogParameterReading(
            Id, assetId, assetCode, assetName,
            parameterId, parameterName, parameterCode, parameterType, unitOfMeasureId,
            value, groupNumber, readingTime, OperationShiftId, shiftLogCheckPointLinkedId);

        _readings.Add(reading);
        _readingEvents.Add(new ShiftLogParameterReadingEventData
        {
            AssetId = assetId,
            ParameterId = parameterId,
            Value = value
        });
    }

    public void UpdateReadingValue(Guid readingId, decimal newValue)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId) ?? throw new DomainException($"Reading with ID {readingId} not found");
        var change = reading.UpdateValue(newValue);
        if (change)
        {
            _readingEvents.Add(new ShiftLogParameterReadingEventData
            {
                AssetId = reading.AssetId,
                ParameterId = reading.ParameterId,
                Value = newValue
            });
        }
    }

    public int GetReadingEventCount()
    {
        return _readingEvents.Count;
    }

    public int RaiseReadingEvents()
    {
        var count = _readingEvents.Count;
        if (count == 0) return 0;

        Raise(new ShiftLogReadingEvent(
            shiftLogId: Id,
            parameterReadings: [.. _readingEvents]
        ));

        _readingEvents.Clear();
        return count;
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
