using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Dtos;

public record AddMaintenancePlan
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public required MaintenancePlanType PlanType { get; set; }

    // For Time-based maintenance plans
    public string? RRule { get; set; }

    // For Parameter-based maintenance plans
    public long? ParameterId { get; set; }
    public decimal? TriggerValue { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public MaintenanceTriggerCondition? TriggerCondition { get; set; }

    // Job steps
    public List<AddMaintenancePlanJobStepDto>? JobSteps { get; set; }

    // Required items (vật tư phụ tùng)
    public List<AddMaintenancePlanRequiredItemDto>? RequiredItems { get; set; }
}

public record AddMaintenancePlanJobStepDto
{
    public required string Name { get; set; }
    public long? OrganizationUnitId { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; }
}

public record AddMaintenancePlanRequiredItemDto
{
    public required long ItemId { get; set; }
    public required decimal Quantity { get; set; }
    public required bool IsRequired { get; set; }
    public string? Note { get; set; }
}
