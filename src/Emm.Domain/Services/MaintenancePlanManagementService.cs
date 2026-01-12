using Emm.Domain.Entities.AssetCatalog;
using Emm.Domain.Exceptions;

namespace Emm.Domain.Services;

/// <summary>
/// Domain service for managing maintenance plans on AssetModel aggregate.
/// Extracted from AssetModel to reduce complexity and separate concerns.
/// </summary>
public class MaintenancePlanManagementService
{
    /// <summary>
    /// Adds a time-based maintenance plan to the asset model.
    /// </summary>
    public void AddTimeBasedMaintenancePlan(
        AssetModel assetModel,
        string name,
        string? description,
        string rrule,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec>? requiredItems = null,
        bool isActive = true)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = MaintenancePlanDefinition.CreateTimeBased(
            assetModelId: assetModel.Id,
            name: name,
            description: description,
            rrule: rrule,
            jobSteps: jobSteps,
            requiredItems: requiredItems,
            isActive: isActive
        );

        assetModel.AddMaintenancePlanInternal(maintenancePlan);

        // RaiseDomainEvent(new MaintenancePlanAddedEvent(assetModel.Id, maintenancePlan.Id, name));
    }

    /// <summary>
    /// Adds a parameter-based maintenance plan to the asset model.
    /// Validates that the parameter exists and marks it as a maintenance parameter.
    /// </summary>
    public void AddParameterBasedMaintenancePlan(
        AssetModel assetModel,
        string name,
        string? description,
        Guid parameterId,
        decimal thresholdValue,
        decimal plusTolerance,
        decimal minusTolerance,
        MaintenanceTriggerCondition triggerCondition = MaintenanceTriggerCondition.GreaterThanOrEqual,
        IReadOnlyCollection<MaintenancePlanJobStepDefinitionSpec>? jobSteps = null,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec>? requiredItems = null,
        bool isActive = true)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var existParameter = assetModel.FindParameter(parameterId);
        if (existParameter == null)
        {
            throw new DomainException($"Parameter {parameterId} is not associated with this AssetModel");
        }

        existParameter.MarkAsMaintenanceParameter();

        var maintenancePlan = MaintenancePlanDefinition.CreateParameterBased(
            assetModelId: assetModel.Id,
            name: name,
            description: description,
            parameterId: parameterId,
            thresholdValue: thresholdValue,
            plusTolerance: plusTolerance,
            minusTolerance: minusTolerance,
            triggerCondition: triggerCondition,
            jobSteps: jobSteps,
            requiredItems: requiredItems,
            isActive: isActive
        );

        assetModel.AddMaintenancePlanInternal(maintenancePlan);

        // RaiseDomainEvent(new MaintenancePlanAddedEvent(assetModel.Id, maintenancePlan.Id, name));
    }

    /// <summary>
    /// Removes a maintenance plan from the asset model.
    /// If it's parameter-based, unmarks the associated parameter.
    /// </summary>
    public void RemoveMaintenancePlan(AssetModel assetModel, Guid maintenancePlanId)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        if (maintenancePlan!.PlanType == MaintenancePlanType.ParameterBased)
        {
            if (maintenancePlan.ParameterBasedTrigger == null)
            {
                throw new DomainException("ParameterBasedTrigger is null for a parameter-based maintenance plan");
            }

            var parameterId = maintenancePlan.ParameterBasedTrigger.ParameterId;
            var associatedParameter = assetModel.FindParameter(parameterId);

            associatedParameter?.UnmarkAsMaintenanceParameter();
        }

        assetModel.RemoveMaintenancePlanInternal(maintenancePlanId);

        // RaiseDomainEvent(new MaintenancePlanRemovedEvent(assetModel.Id, maintenancePlanId));
    }

    /// <summary>
    /// Updates basic maintenance plan properties (name, description, isActive).
    /// </summary>
    public void UpdateMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        string name,
        string? description,
        bool isActive)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.Update(name, description, isActive);

        // RaiseDomainEvent(new MaintenancePlanUpdatedEvent(assetModel.Id, maintenancePlanId, name));
    }

    /// <summary>
    /// Updates a time-based maintenance plan including its recurrence rule.
    /// </summary>
    public void UpdateTimeBasedMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        string name,
        string? description,
        string rrule,
        bool isActive)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateTimeBasedPlan(name, description, rrule, isActive);

        // RaiseDomainEvent(new MaintenancePlanUpdatedEvent(assetModel.Id, maintenancePlanId, name));
    }

    /// <summary>
    /// Updates a parameter-based maintenance plan including trigger conditions.
    /// </summary>
    public void UpdateParameterBasedMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        string name,
        string? description,
        decimal triggerValue,
        decimal plusTolerance,
        decimal minusTolerance,
        MaintenanceTriggerCondition triggerCondition,
        bool isActive)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateParameterBasedPlan(
            name,
            description,
            triggerValue,
            minusTolerance,
            plusTolerance,
            triggerCondition,
            isActive
        );

        // RaiseDomainEvent(new MaintenancePlanUpdatedEvent(assetModel.Id, maintenancePlanId, name));
    }

    /// <summary>
    /// Adds a job step to a maintenance plan.
    /// </summary>
    public void AddJobStepToMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        string stepName,
        Guid? organizationUnitId,
        string? note,
        int order)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.AddJobStep(stepName, organizationUnitId, note, order);

        // RaiseDomainEvent(new MaintenancePlanJobStepAddedEvent(assetModel.Id, maintenancePlanId, stepName));
    }

    /// <summary>
    /// Removes a job step from a maintenance plan.
    /// </summary>
    public void RemoveJobStepFromMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        Guid jobStepId)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.RemoveJobStep(jobStepId);

        // RaiseDomainEvent(new MaintenancePlanJobStepRemovedEvent(assetModel.Id, maintenancePlanId, jobStepId));
    }

    /// <summary>
    /// Updates a job step in a maintenance plan.
    /// </summary>
    public void UpdateJobStepInMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        Guid jobStepId,
        string stepName,
        string? note,
        int order)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateJobStep(jobStepId, stepName, note, order);

        // RaiseDomainEvent(new MaintenancePlanJobStepUpdatedEvent(assetModel.Id, maintenancePlanId, jobStepId, stepName));
    }

    /// <summary>
    /// Adds a required item to a maintenance plan.
    /// </summary>
    public void AddRequiredItemToMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        Guid itemGroupId,
        Guid itemId,
        Guid unitOfMeasureId,
        decimal quantity,
        bool isRequired,
        string? note = null)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));
        DomainGuard.AgainstInvalidForeignKey(itemId, nameof(itemId));
        DomainGuard.AgainstNegative(quantity, nameof(quantity));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.AddRequiredItem(itemGroupId, itemId, unitOfMeasureId, quantity, isRequired, note);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemAddedEvent(assetModel.Id, maintenancePlanId, itemId));
    }

    /// <summary>
    /// Removes a required item from a maintenance plan.
    /// </summary>
    public void RemoveRequiredItemFromMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        Guid requiredItemId)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));
        DomainGuard.AgainstInvalidForeignKey(requiredItemId, nameof(requiredItemId));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.RemoveRequiredItem(requiredItemId);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemRemovedEvent(assetModel.Id, maintenancePlanId, requiredItemId));
    }

    /// <summary>
    /// Updates a required item in a maintenance plan.
    /// </summary>
    public void UpdateRequiredItemInMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        Guid requiredItemId,
        decimal quantity,
        bool isRequired,
        string? note)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));
        DomainGuard.AgainstInvalidForeignKey(requiredItemId, nameof(requiredItemId));
        DomainGuard.AgainstNegative(quantity, nameof(quantity));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.UpdateRequiredItem(requiredItemId, quantity, isRequired, note);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemUpdatedEvent(assetModel.Id, maintenancePlanId, requiredItemId));
    }

    /// <summary>
    /// Synchronizes required items in a maintenance plan.
    /// </summary>
    public void SyncRequiredItemsInMaintenancePlan(
        AssetModel assetModel,
        Guid maintenancePlanId,
        IReadOnlyCollection<MaintenancePlanRequiredItemDefinitionSpec> requiredItemSpecs)
    {
        DomainGuard.AgainstNull(assetModel, nameof(assetModel));
        DomainGuard.AgainstNull(requiredItemSpecs, nameof(requiredItemSpecs));

        var maintenancePlan = assetModel.GetMaintenancePlan(maintenancePlanId);
        DomainGuard.AgainstNotFound(maintenancePlan, nameof(MaintenancePlanDefinition), maintenancePlanId);

        maintenancePlan!.SyncRequiredItems(requiredItemSpecs);

        // RaiseDomainEvent(new MaintenancePlanRequiredItemsSyncedEvent(assetModel.Id, maintenancePlanId));
    }
}
