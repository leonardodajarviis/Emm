using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppMaintenancePlan.Dtos;

public record MaintenancePlanDefinitionResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required MaintenancePlanType PlanType { get; set; }
    public bool IsActive { get; set; }
    public required Guid AssetModelId { get; set; }
    public string? AssetModelName { get; set; }
    public string? RRule { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public string? ModifiedBy { get; set; }

    // For Parameter-based maintenance
    public ParameterBasedMaintenanceTriggerResponse? ParameterBasedTrigger { get; set; }

    // Job steps
    public IReadOnlyCollection<MaintenancePlanJobStepDefinitionResponse> JobSteps { get; set; } = [];

    // Required items (vật tư phụ tùng)
    public IReadOnlyCollection<MaintenancePlanRequiredItemResponse> RequiredItems { get; set; } = [];
}

public record ParameterBasedMaintenanceTriggerResponse
{
    public required Guid Id { get; set; }
    public required Guid ParameterId { get; set; }
    public string? ParameterName { get; set; }
    public required decimal Value { get; set; }
    public required decimal PlusTolerance { get; set; }
    public required decimal MinusTolerance { get; set; }
    public bool IsActive { get; set; }
}

public record MaintenancePlanJobStepDefinitionResponse
{
    public required Guid Id { get; set; }
    public required Guid MaintenancePlanDefinitionId { get; set; }
    public required string Name { get; set; }
    public Guid? OrganizationUnitId { get; set; }
    public string? OrganizationUnitName { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; }
}

public record MaintenancePlanRequiredItemResponse
{
    public required Guid Id { get; set; }
    public required Guid MaintenancePlanDefinitionId { get; set; }
    public required Guid ItemGroupId { get; set; }
    public string? ItemGroupName { get; set; }
    public required Guid ItemId { get; set; }
    public string? ItemName { get; set; }
    public required Guid UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public required decimal Quantity { get; set; }
    public required bool IsRequired { get; set; }
    public string? Note { get; set; }
}

public record MaintenancePlanDefinitionListItemResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required MaintenancePlanType PlanType { get; set; }
    public bool IsActive { get; set; }
    public required Guid AssetModelId { get; set; }
    public string? AssetModelName { get; set; }
    public string? RRule { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public int JobStepsCount { get; set; }
    public int RequiredItemsCount { get; set; }
}
