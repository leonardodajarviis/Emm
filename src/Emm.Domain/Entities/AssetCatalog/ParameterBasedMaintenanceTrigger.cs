namespace Emm.Domain.Entities.AssetCatalog;

public class ParameterBasedMaintenanceTrigger
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid MaintenancePlanDefinitionId { get; private set; }
    public Guid ParameterId { get; private set; }
    public decimal TriggerValue { get; private set; }
    public decimal MinValue { get; private set; }
    public decimal MaxValue { get; private set; }
    public MaintenanceTriggerCondition TriggerCondition { get; private set; }
    public bool IsActive { get; private set; }

    public ParameterBasedMaintenanceTrigger(
        Guid parameterId,
        decimal triggerValue,
        decimal minValue,
        decimal maxValue,
        MaintenanceTriggerCondition triggerCondition = MaintenanceTriggerCondition.GreaterThanOrEqual,
        bool isActive = true)
    {
        ParameterId = parameterId;
        TriggerValue = triggerValue;
        MinValue = minValue;
        MaxValue = maxValue;
        TriggerCondition = triggerCondition;
        IsActive = isActive;
    }

    public bool ShouldTriggerMaintenance(decimal currentValue)
    {
        if (!IsActive)
            return false;

        return TriggerCondition switch
        {
            MaintenanceTriggerCondition.GreaterThan => currentValue > TriggerValue,
            MaintenanceTriggerCondition.GreaterThanOrEqual => currentValue >= TriggerValue,
            MaintenanceTriggerCondition.LessThan => currentValue < TriggerValue,
            MaintenanceTriggerCondition.LessThanOrEqual => currentValue <= TriggerValue,
            MaintenanceTriggerCondition.Equal => currentValue == TriggerValue,
            MaintenanceTriggerCondition.NotEqual => currentValue != TriggerValue,
            MaintenanceTriggerCondition.Between => currentValue >= MinValue && currentValue <= MaxValue,
            MaintenanceTriggerCondition.Outside => currentValue < MinValue || currentValue > MaxValue,
            _ => false
        };
    }

    public void Update(
        decimal triggerValue,
        decimal minValue,
        decimal maxValue,
        MaintenanceTriggerCondition triggerCondition,
        bool isActive)
    {
        TriggerValue = triggerValue;
        MinValue = minValue;
        MaxValue = maxValue;
        TriggerCondition = triggerCondition;
        IsActive = isActive;
    }

    private ParameterBasedMaintenanceTrigger() { } // EF Core constructor
}
