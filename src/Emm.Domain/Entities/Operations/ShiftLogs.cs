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

    // Collections - using backing field pattern for EF Core
    /// <summary>
    /// Danh sách asset liên kết với ShiftLog này
    /// Có thể là 0 (shift-level log), 1 (single asset), hoặc nhiều asset (group)
    /// </summary>
    private readonly List<ShiftLogAsset> _assets;
    public IReadOnlyCollection<ShiftLogAsset> Assets => _assets;

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
        DateTime? endTime = null)
    {
        if (operationShiftId <= 0)
            throw new DomainException("Invalid operation shift ID");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Task name is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Task description is required");


        _assets = [];
        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];

        OperationShiftId = operationShiftId;
        Name = name;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;
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

    #region Asset Management

    /// <summary>
    /// Thêm asset vào ShiftLog - hỗ trợ ghi log cho nhiều asset cùng lúc
    /// </summary>
    public void AddAsset(
        long assetId,
        string assetCode,
        string assetName,
        bool isPrimary = false)
    {
        if (assetId <= 0)
            throw new DomainException("Invalid asset ID");

        if (string.IsNullOrWhiteSpace(assetCode))
            throw new DomainException("Asset code is required");

        if (string.IsNullOrWhiteSpace(assetName))
            throw new DomainException("Asset name is required");

        // Kiểm tra trùng lặp
        if (_assets.Any(a => a.AssetId == assetId))
            throw new DomainException($"Asset {assetCode} already added to this shift log");

        // Nếu đánh dấu là primary, bỏ primary của asset khác
        if (isPrimary)
        {
            foreach (var asset in _assets.Where(a => a.IsPrimary))
            {
                asset.UnmarkAsPrimary();
            }
        }

        var shiftLogAsset = new ShiftLogAsset(Id, assetId, assetCode, assetName, isPrimary);
        _assets.Add(shiftLogAsset);
    }

    /// <summary>
    /// Thêm nhiều assets cùng lúc - cho trường hợp log nhóm thiết bị
    /// </summary>
    public void AddAssets(IEnumerable<(long assetId, string assetCode, string assetName, bool isPrimary)> assets)
    {
        foreach (var (assetId, assetCode, assetName, isPrimary) in assets)
        {
            AddAsset(assetId, assetCode, assetName, isPrimary);
        }
    }

    /// <summary>
    /// Xóa asset khỏi ShiftLog
    /// </summary>
    public void RemoveAsset(long assetId)
    {
        var asset = _assets.FirstOrDefault(a => a.AssetId == assetId);
        if (asset == null)
            throw new DomainException($"Asset with ID {assetId} not found in this shift log");

        _assets.Remove(asset);
    }

    /// <summary>
    /// Đánh dấu asset là primary trong nhóm
    /// </summary>
    public void MarkAssetAsPrimary(long assetId)
    {
        var asset = _assets.FirstOrDefault(a => a.AssetId == assetId);
        if (asset == null)
            throw new DomainException($"Asset with ID {assetId} not found in this shift log");

        // Bỏ primary của các asset khác
        foreach (var a in _assets.Where(a => a.IsPrimary && a.AssetId != assetId))
        {
            a.UnmarkAsPrimary();
        }

        asset.MarkAsPrimary();
    }

    /// <summary>
    /// Lấy danh sách asset IDs trong ShiftLog này
    /// </summary>
    public IEnumerable<long> GetAssetIds()
    {
        return _assets.Select(a => a.AssetId);
    }

    /// <summary>
    /// Kiểm tra xem ShiftLog có chứa asset này không
    /// </summary>
    public bool HasAsset(long assetId)
    {
        return _assets.Any(a => a.AssetId == assetId);
    }

    /// <summary>
    /// Lấy primary asset (nếu có)
    /// </summary>
    public ShiftLogAsset? GetPrimaryAsset()
    {
        return _assets.FirstOrDefault(a => a.IsPrimary);
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
        _assets = [];
        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];
    } // EF Core constructor
}
