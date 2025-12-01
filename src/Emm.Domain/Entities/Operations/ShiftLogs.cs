using Emm.Domain.Abstractions;
using Emm.Domain.Entities.Organization;
using Emm.Domain.Events.Operations;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Entities.Operations;

public class ShiftLog : AggregateRoot, IAuditableEntity
{
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public long Id { get; private set; }
    public long OperationShiftId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime? StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public string? Notes { get; private set; }

    // Collections - using backing field pattern for EF Core
    private readonly List<ShiftLogParameterReading> _readings;
    public IReadOnlyCollection<ShiftLogParameterReading> Readings => _readings;

    private readonly List<ShiftLogCheckpoint> _checkpoints;
    public IReadOnlyCollection<ShiftLogCheckpoint> Checkpoints => _checkpoints;

    private readonly List<ShiftLogEvent> _events;
    public IReadOnlyCollection<ShiftLogEvent> Events => _events;

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


        _readings = [];
        _checkpoints = [];
        _events = [];

        OperationShiftId = operationShiftId;
        Name = name;
        Description = description;
        StartTime = startTime;
        EndTime = endTime;

        // Raise domain event
        Raise(new OperationShiftTaskCreatedEvent(operationShiftId, name));
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

    private ShiftLog()
    {
        _readings = [];
        _checkpoints = [];
        _events = [];
    } // EF Core constructor
}
