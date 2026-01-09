namespace Emm.Domain.Entities.AssetCatalog;

public class ParameterBasedMaintenanceTrigger
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid MaintenancePlanDefinitionId { get; private set; }
    public Guid ParameterId { get; private set; }
    public decimal Value { get; private set; }
    public decimal PlusTolerance { get; private set; }
    public decimal MinusTolerance { get; private set; }
    public MaintenanceTriggerCondition TriggerCondition { get; private set; }
    public bool IsActive { get; private set; }

    public ParameterBasedMaintenanceTrigger(
        Guid parameterId,
        decimal value,
        decimal plusTolerance,
        decimal minusTolerance,
        MaintenanceTriggerCondition triggerCondition = MaintenanceTriggerCondition.GreaterThanOrEqual,
        bool isActive = true)
    {
        ParameterId = parameterId;
        Value = value;
        PlusTolerance = plusTolerance;
        MinusTolerance = minusTolerance;
        TriggerCondition = triggerCondition;
        IsActive = isActive;
    }

    public void Update(
        decimal value,
        decimal plusTolerance,
        decimal minusTolerance,
        MaintenanceTriggerCondition triggerCondition,
        bool isActive)
    {
        Value = value;
        PlusTolerance = plusTolerance;
        MinusTolerance = minusTolerance;
        TriggerCondition = triggerCondition;
        IsActive = isActive;
    }

    private ParameterBasedMaintenanceTrigger() { } // EF Core constructor
}
