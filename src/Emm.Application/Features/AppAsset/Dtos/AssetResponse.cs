using Emm.Application.Features.AppAssetModel.Dtos;

namespace Emm.Application.Features.AppAsset.Dtos;

public class AssetSummaryResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public required long AssetModelId { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}


public class AssetResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public required long AssetCategoryId { get; set; }
    public string? AssetCategoryCode { get; set; }
    public string? AssetCategoryName { get; set; }
    public required long AssetModelId { get; set; }
    public string? AssetModelCode { get; set; }
    public string? AssetModelName { get; set; }
    public required long AssetTypeId { get; set; }
    public string? AssetTypeCode { get; set; }
    public string? AssetTypeName { get; set; }
    public long? OrganizationUnitId { get; set; }
    public string? OrganizationUnitName { get; set; }
    public long? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
    public int Status { get; set; }

    // Maintenance Plan Definitions từ AssetModel
    public IReadOnlyCollection<MaintenancePlanDefinitionResponse> MaintenancePlanDefinitions { get; set; } = [];
}
