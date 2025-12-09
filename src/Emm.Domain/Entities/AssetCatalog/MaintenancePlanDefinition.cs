using Emm.Domain.Abstractions;
using Emm.Domain.ValueObjects;

namespace Emm.Domain.Entities.AssetCatalog;

public class MaintenancePlanDefinition: IAuditableEntity
{
    public long Id { get; private set; }
    public bool IsActive { get; private set; }
    public long AssetModelId { get; private set; }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public MaintenancePlanType PlanType { get; private set; }
    public string? RRule { get; private set; }

    private ParameterBasedMaintenanceTrigger? _parameterBasedTrigger;
    public ParameterBasedMaintenanceTrigger? ParameterBasedTrigger => _parameterBasedTrigger;

    // Job steps (chung cho cả 2 loại)
    private readonly List<MaintenancePlanJobStepDefinition> _jobSteps;
    public IReadOnlyCollection<MaintenancePlanJobStepDefinition> JobSteps => _jobSteps;

    // Danh sách vật tư phụ tùng cần thiết
    private readonly List<MaintenancePlanRequiredItem> _requiredItems;
    public IReadOnlyCollection<MaintenancePlanRequiredItem> RequiredItems => _requiredItems;

    public AuditMetadata Audit { get; private set; } = null!;
    public void SetAudit(AuditMetadata audit) => Audit = audit;

    private MaintenancePlanDefinition()
    {
        _jobSteps = [];
        _requiredItems = [];
    }

    // Constructor cho Time-based maintenance plan
    public MaintenancePlanDefinition(
        long assetModelId,
        string name,
        string? description,
        string rrule,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec>? requiredItems = null,
        bool isActive = true)
    {
        _jobSteps = [];
        _requiredItems = [];
        // EfJobSteps = _jobSteps;

        AssetModelId = assetModelId;
        Name = name;
        Description = description;
        PlanType = MaintenancePlanType.TimeBased;
        RRule = rrule;
        IsActive = isActive;


        // Thêm job steps nếu được cung cấp
        if (jobSteps != null)
        {
            foreach (var step in jobSteps)
            {
                AddJobStep(
                    name: step.Name,
                    organizationUnitId: step.OrganizationUnitId,
                    note: step.Note,
                    order: step.Order
                );
            }
        }

        if (requiredItems != null)
        {
            foreach (var item in requiredItems)
            {
                AddRequiredItem(
                    itemId: item.ItemId,
                    quantity: item.Quantity,
                    isRequired: item.IsRequired,
                    note: item.Note
                );
            }
        }
    }

    // Constructor cho Parameter-based maintenance plan
    public MaintenancePlanDefinition(
        long assetModelId,
        string name,
        string? description,
        long parameterId,
        decimal triggerValue,
        decimal minValue,
        decimal maxValue,
        MaintenanceTriggerCondition triggerCondition = MaintenanceTriggerCondition.GreaterThanOrEqual,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec>? requiredItems = null,
        bool isActive = true)
    {
        _jobSteps = [];
        _requiredItems = [];
        // EfJobSteps = _jobSteps;

        AssetModelId = assetModelId;
        Name = name;
        Description = description;
        PlanType = MaintenancePlanType.ParameterBased;
        IsActive = isActive;

        _parameterBasedTrigger = new ParameterBasedMaintenanceTrigger(
            parameterId: parameterId,
            triggerValue: triggerValue,
            minValue: minValue,
            maxValue: maxValue,
            triggerCondition: triggerCondition,
            isActive: isActive
        );

        // Thêm job steps nếu được cung cấp
        if (jobSteps != null)
        {
            foreach (var step in jobSteps)
            {
                AddJobStep(
                    name: step.Name,
                    organizationUnitId: step.OrganizationUnitId,
                    note: step.Note,
                    order: step.Order
                );
            }
        }

        if (requiredItems != null)
        {
            foreach (var item in requiredItems)
            {
                AddRequiredItem(
                    itemId: item.ItemId,
                    quantity: item.Quantity,
                    isRequired: item.IsRequired,
                    note: item.Note
                );
            }
        }
    }

    // Constructor với job steps
    public MaintenancePlanDefinition(
        long assetModelId,
        string name,
        string? description,
        MaintenancePlanType planType,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        bool isActive = true)
    {
        _jobSteps = [];
        _requiredItems = [];
        // EfJobSteps = _jobSteps;

        AssetModelId = assetModelId;
        Name = name;
        Description = description;
        PlanType = planType;
        IsActive = isActive;


        if (jobSteps is not null)
        {
            foreach (var step in jobSteps)
            {
                AddJobStep(
                    name: step.Name,
                    organizationUnitId: step.OrganizationUnitId,
                    note: step.Note,
                    order: step.Order
                );
            }
        }
    }

    public void Update(string name, string? description, bool isActive)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
    }

    // Update for Time-based maintenance plan
    public void UpdateTimeBasedPlan(
        string name,
        string? description,
        string rrule,
        bool isActive)
    {
        if (PlanType != MaintenancePlanType.TimeBased)
            throw new InvalidOperationException("Cannot update time-based settings on non-time-based maintenance plan");

        Name = name;
        Description = description;
        RRule = rrule;
        IsActive = isActive;
    }

    // Update for Parameter-based maintenance plan
    public void UpdateParameterBasedPlan(
        string name,
        string? description,
        decimal triggerValue,
        decimal minValue,
        decimal maxValue,
        MaintenanceTriggerCondition triggerCondition,
        bool isActive)
    {
        if (PlanType != MaintenancePlanType.ParameterBased)
            throw new InvalidOperationException("Cannot update parameter-based settings on non-parameter-based maintenance plan");

        if (_parameterBasedTrigger == null)
            throw new InvalidOperationException("Parameter-based trigger is not initialized");

        Name = name;
        Description = description;
        IsActive = isActive;

        _parameterBasedTrigger.Update(
            triggerValue: triggerValue,
            minValue: minValue,
            maxValue: maxValue,
            triggerCondition: triggerCondition,
            isActive: isActive
        );
    }

    // Methods cho Parameter-based maintenance

    public void AddJobStep(string name, long? organizationUnitId, string? note, int order)
    {
        var jobStep = new MaintenancePlanJobStepDefinition(name, organizationUnitId, note, order);
        _jobSteps.Add(jobStep);
    }

    public void RemoveJobStep(long jobStepId)
    {
        var jobStep = _jobSteps.FirstOrDefault(js => js.Id == jobStepId);
        if (jobStep != null)
        {
            _jobSteps.Remove(jobStep);
        }
    }

    public void UpdateJobStep(long jobStepId, string name, string? note, int order)
    {
        var jobStep = _jobSteps.FirstOrDefault(js => js.Id == jobStepId);
        if (jobStep == null)
            throw new InvalidOperationException($"Job step with ID {jobStepId} not found");

        jobStep.Update(name, note, order);
    }

    public void SyncJobSteps(IReadOnlyCollection<JobStepSpec> jobStepSpecs)
    {
        // Xóa các job steps không còn trong danh sách mới
        var incomingIds = jobStepSpecs
            .Where(spec => spec.Id.HasValue)
            .Select(spec => spec.Id!.Value)
            .ToList();

        var stepsToRemove = _jobSteps
            .Where(step => !incomingIds.Contains(step.Id))
            .ToList();

        foreach (var step in stepsToRemove)
        {
            _jobSteps.Remove(step);
        }

        // Cập nhật hoặc thêm mới
        foreach (var spec in jobStepSpecs)
        {
            if (spec.Id.HasValue)
            {
                // Update existing
                var existingStep = _jobSteps.FirstOrDefault(js => js.Id == spec.Id.Value);
                if (existingStep != null)
                {
                    existingStep.Update(spec.Name, spec.Note, spec.Order);
                }
            }
            else
            {
                // Add new
                var newStep = new MaintenancePlanJobStepDefinition(
                    spec.Name,
                    spec.OrganizationUnitId,
                    spec.Note,
                    spec.Order
                );
                _jobSteps.Add(newStep);
            }
        }
    }

    // Methods cho Required Items (Vật tư phụ tùng)

    public void AddRequiredItem(long itemId, decimal quantity, bool isRequired, string? note = null)
    {
        var requiredItem = new MaintenancePlanRequiredItem(itemId, quantity, isRequired, note);
        _requiredItems.Add(requiredItem);
    }

    public void RemoveRequiredItem(long requiredItemId)
    {
        var requiredItem = _requiredItems.FirstOrDefault(ri => ri.Id == requiredItemId);
        if (requiredItem != null)
        {
            _requiredItems.Remove(requiredItem);
        }
    }

    public void UpdateRequiredItem(long requiredItemId, decimal quantity, bool isRequired, string? note)
    {
        var requiredItem = _requiredItems.FirstOrDefault(ri => ri.Id == requiredItemId);
        if (requiredItem == null)
            throw new InvalidOperationException($"Required item with ID {requiredItemId} not found");

        requiredItem.Update(quantity, isRequired, note);
    }

    public void SyncRequiredItems(IReadOnlyCollection<RequiredItemSpec> requiredItemSpecs)
    {
        // Xóa các vật tư không còn trong danh sách mới
        var incomingIds = requiredItemSpecs
            .Where(spec => spec.Id.HasValue)
            .Select(spec => spec.Id!.Value)
            .ToList();

        var itemsToRemove = _requiredItems
            .Where(item => !incomingIds.Contains(item.Id))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            _requiredItems.Remove(item);
        }

        // Cập nhật hoặc thêm mới
        foreach (var spec in requiredItemSpecs)
        {
            if (spec.Id.HasValue)
            {
                // Update existing
                var existingItem = _requiredItems.FirstOrDefault(ri => ri.Id == spec.Id.Value);
                if (existingItem != null)
                {
                    existingItem.Update(spec.Quantity, spec.IsRequired, spec.Note);
                }
            }
            else
            {
                // Add new
                var newItem = new MaintenancePlanRequiredItem(
                    spec.ItemId,
                    spec.Quantity,
                    spec.IsRequired,
                    spec.Note
                );
                _requiredItems.Add(newItem);
            }
        }
    }
}

public record MaintenancePlanJobStepDefinitionSpec(
    string Name,
    long? OrganizationUnitId,
    string? Note,
    int Order
);

public record MaintenancePlanRequiredItemDefinitionSpec(
    long ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);

public record JobStepSpec(
    long? Id,
    string Name,
    long? OrganizationUnitId,
    string? Note,
    int Order
);

public record RequiredItemSpec(
    long? Id,
    long ItemId,
    decimal Quantity,
    bool IsRequired,
    string? Note
);
