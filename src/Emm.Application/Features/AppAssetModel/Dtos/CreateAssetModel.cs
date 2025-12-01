using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Dtos;

public class CreateAssetModel
{
    public required string Name { get; set; }

    public string? Description { get; set; }

    public string? Notes { get; set; }
    public long? ParentId { get; set; }
    public long? AssetCategoryId { get; set; }
    public long? AssetTypeId { get; set; }
    public List<long>? ParameterIds { get; set; }
    public List<CreateMaintenancePlanDefinitionDto>? MaintenancePlanDefinitions { get; set; }
    public List<CreateAssetModelImageDto>? Images { get; set; }
    public Guid? ThumbnailFileId { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateMaintenancePlanDefinitionDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public MaintenancePlanType PlanType { get; set; }
    public bool IsActive { get; set; } = true;

    // For Time-based maintenance plans
    public string? RRule { get; set; }

    // For Parameter-based maintenance plans
    public long? ParameterId { get; set; }
    public decimal? TriggerValue { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public MaintenanceTriggerCondition? TriggerCondition { get; set; }

    // Job steps (common for both types)
    public List<MaintenancePlanJobStepDefinitionDto>? JobSteps { get; set; }
}

public class MaintenancePlanJobStepDefinitionDto
{
    public required string Name { get; set; }
    public long? OrganizationUnitId { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; }
}

public class CreateAssetModelImageDto
{
    public Guid FileId { get; set; }
}
