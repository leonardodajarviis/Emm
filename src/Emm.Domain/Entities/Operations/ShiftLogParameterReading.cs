namespace Emm.Domain.Entities.Operations;

/// <summary>
/// Giá trị đọc của tham số thiết bị trong ca vận hành
/// </summary>
public class ShiftLogParameterReading
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid? ShiftLogCheckpointLinkedId {get; private set;}
    public Guid ShiftLogId { get; private set; }
    public Guid AssetId { get; private set; }
    public string AssetCode { get; private set; } = null!;
    public string AssetName { get; private set; } = null!;
    public Guid ParameterId { get; private set; }
    public string ParameterName { get; private set; } = null!;
    public string ParameterCode { get; private set; } = null!;
    public decimal Value { get; private set; }
    public string? StringValue { get; private set; }
    public Guid UnitOfMeasureId { get; private set; }
    public DateTime ReadingTime { get; private set; }
    public string? Notes { get; private set; }
    public bool IsLooked { get; private set; }

    public ShiftLogParameterReading(
        Guid operationTaskId,
        Guid assetId,
        string assetCode,
        string assetName,
        Guid parameterId,
        string parameterName,
        string parameterCode,
        Guid unitOfMeasureId,
        decimal value,
        Guid? shiftLogCheckpointLinkedId = null,
        string? stringValue = null,
        string? notes = null)
    {
        ShiftLogId = operationTaskId;
        AssetId = assetId;
        AssetCode = assetCode;
        AssetName = assetName;
        ParameterId = parameterId;
        ParameterName = parameterName;
        ParameterCode = parameterCode;
        Value = value;
        StringValue = stringValue;
        UnitOfMeasureId = unitOfMeasureId;
        ReadingTime = DateTime.UtcNow;
        Notes = notes;
        ShiftLogCheckpointLinkedId = shiftLogCheckpointLinkedId;
    }

    public void Locked()
    {
        IsLooked = true;
    }

    public void UpdateValue(decimal value)
    {
        Value = value;
    }

    public void SetWarning(string reason)
    {
        Notes = reason;
    }

    public void SetCritical(string reason)
    {
        Notes = reason;
    }

    private ShiftLogParameterReading() { } // EF Core constructor
}
