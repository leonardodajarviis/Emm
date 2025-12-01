using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Domain.Entities.AssetCatalog;

namespace Emm.Application.Features.AppAssetModel.Dtos;

public record AssetModelDetailResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public long? ParentId { get; set; }
    public long? AssetCategoryId { get; set; }
    public string? AssetCategoryName => AssetCategory?.Name;
    public Guid? ThumbnailFileId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public long? AssetTypeId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties with full details
    public AssetModelParentResponse? Parent { get; set; }
    public AssetCategoryResponse? AssetCategory { get; set; }
    public AssetTypeResponse? AssetType { get; set; }
    public IReadOnlyCollection<AssetParameterResponse> Parameters { get; set; } = [];
    public IReadOnlyCollection<MaintenancePlanDefinitionResponse> MaintenancePlanDefinitions { get; set; } = [];
    public IReadOnlyCollection<AssetModelImageResponse> Images { get; set; } = [];
}

public record AssetModelParentResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}

public record AssetCategoryResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}

public record AssetTypeResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}

public record AssetModelOperatingParameterResponse
{
    public required long AssetParameterId { get; set; }
    public AssetParameterResponse? AssetParameter { get; set; }
}

public record MaintenancePlanDefinitionResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required MaintenancePlanType PlanType { get; set; }
    public bool IsActive { get; set; }
    public long AssetModelId { get; set; }
    public string? RRule { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // For Parameter-based maintenance
    public ParameterBasedMaintenanceTriggerResponse? ParameterBasedTrigger { get; set; }

    // Job steps
    public IReadOnlyCollection<MaintenancePlanJobStepDefinitionResponse> JobSteps { get; set; } = [];

    // Required items (vật tư phụ tùng)
    public IReadOnlyCollection<MaintenancePlanRequiredItemResponse> RequiredItems { get; set; } = [];
}

public record ParameterBasedMaintenanceTriggerResponse
{
    public required long Id { get; set; }
    public required long ParameterId { get; set; }
    public required decimal TriggerValue { get; set; }
    public required decimal MinValue { get; set; }
    public required decimal MaxValue { get; set; }
    public required MaintenanceTriggerCondition TriggerCondition { get; set; }
    public bool IsActive { get; set; }
}

public record OrganizationUnitResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}

public record OrganizationUnitLevelResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Level { get; set; }
}

public record MaintenancePlanJobStepDefinitionResponse
{
    public required long Id { get; set; }
    public long MaintenancePlanDefinitionId { get; set; }
    public required string Name { get; set; }
    public long? OrganizationUnitId { get; set; }
    public OrganizationUnitResponse? OrganizationUnit { get; set; }
    public string? Note { get; set; }
    public int Order { get; set; }
}

public record AssetModelImageResponse
{
    public required Guid FileId { get; set; }
    public required string FilePath { get; set; }
    public string? Url { get; set; }
}

public record MaintenancePlanRequiredItemResponse
{
    public required long Id { get; set; }
    public long MaintenancePlanDefinitionId { get; set; }
    public required long ItemId { get; set; }
    public required decimal Quantity { get; set; }
    public required bool IsRequired { get; set; }
    public string? Note { get; set; }
}
