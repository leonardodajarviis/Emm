using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Operations.BusinessRules;
using Emm.Domain.Events.ShiftLogs;
using Emm.Domain.Exceptions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.Operations;

public class ShiftLog : AggregateRoot, IAuditableEntity
{
    public int LogOrder { get; private set; }
    public Guid OperationShiftId { get; private set; }
    public string Batch { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime StartTime { get; private set; }
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
        string batch,
        Guid operationShiftId,
        string name,
        DateTime startTime,
        DateTime? endTime,
        Guid? assetId,
        Guid? boxId,
        Guid? locationId,
        string? locationName)
    {
        DomainGuard.AgainstBusinessRule(
            assetId.HasValue && boxId.HasValue,
            ShiftLogRules.CannotSetBothAssetAndBox,
            "Không thể thiết lập cả AssetId và BoxId cùng lúc.");

        DomainGuard.AgainstBusinessRule(
            !assetId.HasValue && !boxId.HasValue,
            ShiftLogRules.BoxOrAssetMustExist,
            "Phải thiết lập AssetId hoặc BoxId."
        );

        DomainGuard.AgainstBusinessRule(
            endTime.HasValue && endTime < startTime,
            ShiftLogRules.EndTimeCannotBeBeforeStartTime,
            "Thời gian kết thúc không thể trước thời gian bắt đầu."
        );


        _readings = [];
        _checkpoints = [];
        _events = [];
        _items = [];

        Batch = batch;
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
        DateTime endTime)
    {
        var ev = DomainGuard.AgainstNotFound(
            () => _events.FirstOrDefault(e => e.Id == eventId),
            $"Sự kiện với ID {eventId} không tồn tại trong nhật ký ca.");

        ev.Update(eventType, startTime, endTime);
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
        var reading = DomainGuard.AgainstNotFound(
            () => _readings.FirstOrDefault(r => r.Id == readingId),
            $"Reading với ID {readingId} không tồn tại trong nhật ký ca.");

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
        var checkpoint = DomainGuard.AgainstNotFound(
            () => _checkpoints.FirstOrDefault(c => c.Id == checkpointId),
            $"Checkpoint với ID {checkpointId} không tồn tại trong nhật ký ca.");

        checkpoint!.MakeAttachedMaterial(itemId, itemCode, itemName);
    }

    public void UpdateLocationInCheckpoint(Guid checkpointId, Guid locationId, string locationName)
    {
        var checkpoint = DomainGuard.AgainstNotFound(
            () => _checkpoints.FirstOrDefault(c => c.Id == checkpointId),
            $"Checkpoint với ID {checkpointId} không tồn tại trong nhật ký ca.");

        if (checkpoint!.LocationId == locationId) return;

        checkpoint.UpdateLocation(locationId, locationName);
    }

    /// <summary>
    /// Xóa checkpoint theo ID
    /// </summary>
    public void RemoveCheckpoint(Guid checkpointId)
    {
        var checkpoint = DomainGuard.AgainstNotFound(
            () => _checkpoints.FirstOrDefault(c => c.Id == checkpointId),
            $"Checkpoint với ID {checkpointId} không tồn tại trong nhật ký ca.");

        _checkpoints.Remove(checkpoint);
    }

    /// <summary>
    /// Ghi nhận sự kiện: nghỉ ca, sự cố, ghi chú quan trọng
    /// </summary>
    public void RecordEvent(
        ShiftLogEventType eventType,
        DateTime startTime,
        DateTime endTime)
    {

        var @event = new ShiftLogEvent(Id, eventType, startTime, endTime);
        _events.Add(@event);
    }

    /// <summary>
    /// Xóa event theo ID
    /// </summary>
    public void RemoveEvent(Guid eventId)
    {
        var evt = DomainGuard.AgainstNotFound(
            () => _events.FirstOrDefault(e => e.Id == eventId),
            $"Sự kiện với ID {eventId} không tồn tại trong nhật ký ca.");

        _events.Remove(evt);
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
        Guid? assetId,
        string? assetCode,
        string? assetName,
        Guid? unitOfMeasureId,
        string? unitOfMeasureCode,
        string? unitOfMeasureName)
    {
        var item = new ShiftLogItem(
            Id, warehouseIssueSlipId, itemId, itemCode, itemName, quantity,
            assetId, assetCode, assetName,
            unitOfMeasureId, unitOfMeasureCode, unitOfMeasureName);

        _items.Add(item);
    }


    /// <summary>
    /// Xóa item theo ID
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        var item = DomainGuard.AgainstNotFound(
            () => _items.FirstOrDefault(i => i.Id == itemId),
            $"Item với ID {itemId} không tồn tại trong nhật ký ca.");

        _items.Remove(item!);
    }

    public void UpdateItemQuantity(Guid itemId, decimal newQuantity)
    {
        var item = DomainGuard.AgainstNotFound(
            () => _items.FirstOrDefault(i => i.Id == itemId),
            $"Item với ID {itemId} không tồn tại trong nhật ký ca."
        );

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
