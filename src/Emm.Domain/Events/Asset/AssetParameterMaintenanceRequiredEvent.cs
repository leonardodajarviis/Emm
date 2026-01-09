using Emm.Domain.Abstractions;

namespace Emm.Domain.Events.Asset;

public record AssetParameterMaintenanceRequiredEvent : IDeferredDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;

    public Guid AssetId { get; }
    public Guid ParameterId { get; }
    public Guid MaintenancePlanId { get; }
    public decimal CurrentValue { get; }
    public decimal ThresholdValue { get; }
    public decimal PlusTolerance { get; }
    public decimal MinusTolerance { get; }
    public AssetParameterMaintenanceRequiredEvent(
        Guid assetId,
        Guid parameterId,
        Guid maintenancePlanId,
        decimal currentValue,
        decimal thresholdValue,
        decimal plusTolerance,
        decimal minusTolerance)
    {
        AssetId = assetId;
        ParameterId = parameterId;
        CurrentValue = currentValue;
        ThresholdValue = thresholdValue;
        PlusTolerance = plusTolerance;
        MinusTolerance = minusTolerance;
        MaintenancePlanId = maintenancePlanId;
    }
}
