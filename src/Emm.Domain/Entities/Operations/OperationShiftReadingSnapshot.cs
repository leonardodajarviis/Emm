namespace Emm.Domain.Entities.Operations;

public class OperationShiftReadingSnapshot
{
    public Guid OperationShiftId { get; private set; }
    public Guid AssetId { get; private set; }
    public Guid ParameterId { get; private set; }
    public decimal Value { get; private set; }

    public OperationShiftReadingSnapshot(
        Guid operationShiftId,
        Guid assetId,
        Guid parameterId,
        decimal value)
    {
        OperationShiftId = operationShiftId;
        AssetId = assetId;
        ParameterId = parameterId;
        Value = value;
    }

}
