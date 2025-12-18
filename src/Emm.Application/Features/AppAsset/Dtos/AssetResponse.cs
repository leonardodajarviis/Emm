using Emm.Application.Features.AppAssetModel.Dtos;

namespace Emm.Application.Features.AppAsset.Dtos;

public class AssetSummaryResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public required Guid AssetModelId { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ModifiedAt { get; set; }
}


public class AssetResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public required Guid AssetCategoryId { get; set; }
    public string? AssetCategoryCode { get; set; }
    public string? AssetCategoryName { get; set; }
    public required Guid AssetModelId { get; set; }
    public string? AssetModelCode { get; set; }
    public string? AssetModelName { get; set; }
    public required Guid AssetTypeId { get; set; }
    public string? AssetTypeCode { get; set; }
    public string? AssetTypeName { get; set; }
    public Guid? OrganizationUnitId { get; set; }
    public string? OrganizationUnitName { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime? ModifiedAt { get; set; }
    public int Status { get; set; }

    // Maintenance Plan Definitions từ AssetModel
    public IReadOnlyCollection<MaintenancePlanDefinitionResponse> MaintenancePlanDefinitions { get; set; } = [];
}
