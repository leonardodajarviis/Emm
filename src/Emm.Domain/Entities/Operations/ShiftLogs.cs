using Emm.Domain.Abstractions;
using Emm.Domain.Events.Operations;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

public class ShiftLog : AggregateRoot, IAuditableEntity
{
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public long Id { get; private set; }
    public int LogOrder { get; private set; }
    public long OperationShiftId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>
    /// Asset ID cho trường hợp ghi log cho 1 asset cụ thể
    /// Null nếu là shift-level log hoặc ghi log theo group
    /// </summary>
    public long? AssetId { get; private set; }

    /// <summary>
    /// Group ID cho trường hợp ghi log cho nhiều assets theo group
    /// Null nếu là shift-level log hoặc ghi log cho 1 asset cụ thể
    /// </summary>
    public long? GroupId { get; private set; }

    private readonly List<ShiftLogParameterReading> _readings;
    public IReadOnlyCollection<ShiftLogParameterReading> Readings => _readings;

    private readonly List<ShiftLogCheckpoint> _checkpoints;
    public IReadOnlyCollection<ShiftLogCheckpoint> Checkpoints => _checkpoints;

    private readonly List<ShiftLogEvent> _events;
    public IReadOnlyCollection<ShiftLogEvent> Events => _events;

    private readonly List<ShiftLogItem> _items;
    public IReadOnlyCollection<ShiftLogItem> Items => _items;

    public ShiftLog(
        long operationShiftId,
        string name,
        string description,
        DateTime startTime,
        DateTime? endTime = null,
        long? assetId = null,
        long? groupId = null)
    {
        if (operationShiftId <= 0)
            throw new DomainException("Invalid operation shift ID");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Task name is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Task description is required");

        // Validation: Không được set cả AssetId và GroupId cùng lúc
        if (assetId.HasValue && groupId.HasValue)
            throw new DomainException("Cannot set both AssetId and GroupId. Choose either single asset or group.");

        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];

        OperationShiftId = operationShiftId;
        Name = name;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
        AssetId = assetId;
        GroupId = groupId;
    }

    public void UpdateStartTime(DateTime startTime)
    {
        StartTime = startTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEndTime(DateTime endTime)
    {
        if (endTime < StartTime)
            throw new DomainException("End time cannot be before start time");

        EndTime = endTime;
        UpdatedAt = DateTime.UtcNow;
    }

    #region Asset and Group Management

    /// <summary>
    /// Gán ShiftLog cho một asset cụ thể
    /// </summary>
    public void AssignToAsset(long assetId)
    {
        if (assetId <= 0)
            throw new DomainException("Invalid asset ID");

        if (GroupId.HasValue)
            throw new DomainException("Cannot assign to asset when already assigned to a group");

        AssetId = assetId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gán ShiftLog cho một group (nhiều assets)
    /// </summary>
    public void AssignToGroup(long groupId)
    {
        if (groupId <= 0)
            throw new DomainException("Invalid group ID");

        if (AssetId.HasValue)
            throw new DomainException("Cannot assign to group when already assigned to an asset");

        GroupId = groupId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Bỏ gán asset/group (chuyển về shift-level log)
    /// </summary>
    public void UnassignAssetOrGroup()
    {
        AssetId = null;
        GroupId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kiểm tra xem ShiftLog có gắn với asset/group không
    /// </summary>
    public bool IsAssigned()
    {
        return AssetId.HasValue || GroupId.HasValue;
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
    public bool IsAssignedToGroup()
    {
        return GroupId.HasValue;
    }

    #endregion

    public void LockReading(long readingId)
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
        long assetId,
        string assetCode,
        string assetName,
        long parameterId,
        string parameterName,
        string parameterCode,
        decimal value,
        string unit,
        Guid? shiftLogCheckPointLinkedId = null)
    {
        if (assetId <= 0)
            throw new DomainException("Invalid asset ID");

        if (parameterId <= 0)
            throw new DomainException("Invalid parameter ID");


        var reading = new ShiftLogParameterReading(
            Id, assetId, assetCode, assetName,
            parameterId, parameterName, parameterCode,
            value, unit, shiftLogCheckPointLinkedId);

        _readings.Add(reading);
    }

    /// <summary>
    /// Thêm nhiều readings cùng lúc
    /// </summary>
    public void AddReadings(IEnumerable<ShiftLogParameterReading> readings)
    {
        foreach (var reading in readings)
        {
            _readings.Add(reading);
        }
    }

    /// <summary>
    /// Lấy reading theo ID
    /// </summary>
    public ShiftLogParameterReading GetReading(long readingId)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId);
        if (reading == null)
            throw new DomainException($"Reading with ID {readingId} not found");

        return reading;
    }

    /// <summary>
    /// Xóa reading theo ID
    /// </summary>
    public void RemoveReading(long readingId)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId);
        if (reading == null)
            throw new DomainException($"Reading with ID {readingId} not found");

        _readings.Remove(reading);
    }

    public void UpdateReadingValue(long readingId, decimal newValue)
    {
        var reading = _readings.FirstOrDefault(r => r.Id == readingId) ?? throw new DomainException($"Reading with ID {readingId} not found");
        reading.UpdateValue(newValue);
    }

    public void AddCheckpoint(Guid linkedId, string name, long locationId, string locationName, bool isWithAttachedMaterial = false, long? itemId = null, string? itemCode = null, string? itemName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Checkpoint name is required");

        var checkpoint = new ShiftLogCheckpoint(Id, linkedId, name, locationId, locationName, isWithAttachedMaterial, itemId, itemCode, itemName);
        _checkpoints.Add(checkpoint);
    }

    /// <summary>
    /// Thêm nhiều checkpoints cùng lúc
    /// </summary>
    public void AddCheckpoints(IEnumerable<(Guid linkedId, string name, long locationId, string locationName, bool isWithAttachedMaterial, long? itemId, string? itemCode, string? itemName)> checkpoints)
    {
        foreach (var (linkedId, name, locationId, locationName, isWithAttachedMaterial, itemId, itemCode, itemName) in checkpoints)
        {
            AddCheckpoint(linkedId, name, locationId, locationName, isWithAttachedMaterial, itemId, itemCode, itemName);
        }
    }

    /// <summary>
    /// Lấy checkpoint theo ID để thao tác
    /// </summary>
    public ShiftLogCheckpoint GetCheckpoint(long checkpointId)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint == null)
            throw new DomainException($"Checkpoint with ID {checkpointId} not found");

        return checkpoint;
    }

    /// <summary>
    /// Xóa checkpoint theo ID
    /// </summary>
    public void RemoveCheckpoint(long checkpointId)
    {
        var checkpoint = _checkpoints.FirstOrDefault(c => c.Id == checkpointId);
        if (checkpoint == null)
            throw new DomainException($"Checkpoint with ID {checkpointId} not found");

        _checkpoints.Remove(checkpoint);
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Task name is required");

        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Task description is required");

        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ghi nhận sự kiện: nghỉ ca, sự cố, ghi chú quan trọng
    /// </summary>
    public void RecordEvent(
        ShiftLogEventType eventType,
        DateTime startTime)
    {

        var statusHistory = new ShiftLogEvent(Id, eventType, startTime);

        _events.Add(statusHistory);
    }

    /// <summary>
    /// Ghi nhận nhiều sự kiện cùng lúc
    /// </summary>
    public void RecordEvents(IEnumerable<(ShiftLogEventType eventType, DateTime startTime)> events)
    {
        foreach (var evt in events)
        {
            RecordEvent(evt.eventType, evt.startTime);
        }
    }

    /// <summary>
    /// Kết thúc sự kiện đang mở (nghỉ ca, sự cố)
    /// </summary>
    public void EndEvent(long eventId, DateTime endTime)
    {
        var statusHistory = _events.FirstOrDefault(h => h.Id == eventId);
        if (statusHistory == null)
            throw new DomainException($"Event with ID {eventId} not found");

        statusHistory.EndEvent(endTime);
    }

    /// <summary>
    /// Lấy event theo ID
    /// </summary>
    public ShiftLogEvent GetEvent(long eventId)
    {
        var statusHistory = _events.FirstOrDefault(h => h.Id == eventId);
        if (statusHistory == null)
            throw new DomainException($"Event with ID {eventId} not found");

        return statusHistory;
    }

    /// <summary>
    /// Xóa event theo ID
    /// </summary>
    public void RemoveEvent(long eventId)
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
        long itemId,
        string itemName,
        decimal quantity,
        long? assetId = null,
        string? assetCode = null,
        string? assetName = null,
        long? unitOfMeasureId = null,
        string? unitOfMeasureName = null)
    {
        if (itemId <= 0)
            throw new DomainException("Invalid item ID");

        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Item name is required");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var item = new ShiftLogItem(
            Id, itemId, itemName, quantity,
            assetId, assetCode, assetName,
            unitOfMeasureId, unitOfMeasureName);

        _items.Add(item);
    }

    /// <summary>
    /// Thêm nhiều items cùng lúc
    /// </summary>
    public void AddItems(IEnumerable<(long itemId, string itemName, decimal quantity, long? assetId, string? assetCode, string? assetName, long? unitOfMeasureId, string? unitOfMeasureName)> items)
    {
        foreach (var (itemId, itemName, quantity, assetId, assetCode, assetName, unitOfMeasureId, unitOfMeasureName) in items)
        {
            AddItem(itemId, itemName, quantity, assetId, assetCode, assetName, unitOfMeasureId, unitOfMeasureName);
        }
    }

    /// <summary>
    /// Lấy item theo ID
    /// </summary>
    public ShiftLogItem GetItem(long itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException($"Item with ID {itemId} not found");

        return item;
    }

    /// <summary>
    /// Xóa item theo ID
    /// </summary>
    public void RemoveItem(long itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException($"Item with ID {itemId} not found");

        _items.Remove(item);
    }

    /// <summary>
    /// Xóa tất cả items
    /// </summary>
    public void ClearItems()
    {
        _items.Clear();
    }

    private ShiftLog()
    {
        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];
    } // EF Core constructor
}
