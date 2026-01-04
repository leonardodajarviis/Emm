using Emm.Application.Features.AppAssetModel.Dtos;

namespace Emm.Application.Features.AppAsset.Dtos;

public record AssetSummaryResponse
{
    public required Guid Id { get;init; }
    public required string Code { get; init; }
    public required string DisplayName { get; init; }
    public required Guid AssetModelId { get; init; }
    public string? Description { get; init; }
    public required string Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? ModifiedAt { get; init; }
}


public class AssetResponse
{
    public required Guid Id { get; init; }
    public bool IsCodeGenerated { get; init; }
    public required string Code { get; init; }
    public required string DisplayName { get; init; }
    public required Guid AssetCategoryId { get; init; }
    public string? AssetCategoryCode { get; init; }
    public string? AssetCategoryName { get; init; }
    public required Guid AssetModelId { get; init; }
    public string? AssetModelCode { get; init; }
    public string? AssetModelName { get; init; }
    public required Guid AssetTypeId { get; init; }
    public string? AssetTypeCode { get; init; }
    public string? AssetTypeName { get; init; }
    public Guid? OrganizationUnitId { get; init; }
    public string? OrganizationUnitName { get; init; }
    public Guid? LocationId { get; init; }
    public string? LocationName { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public required string Status { get; init; }

    // Maintenance Plan Definitions từ AssetModel
    public IReadOnlyCollection<MaintenancePlanDefinitionResponse> MaintenancePlanDefinitions { get; init; } = [];

    public IReadOnlyCollection<AssetParameterResponse> Parameters { get; init; } = [];
}


public record AssetParameterResponse
{
    public Guid AssetId { get; init; }
    public Guid ParameterId { get; init; }
    public string? ParameterCode { get; init; }
    public string? ParameterName { get; init; }
    public string? ParameterUnit { get; init; }
    public decimal CurrentValue { get; init; }
    public bool IsMaintenanceParameter { get; init; }
    public decimal ValueToMaintenance { get; init;}
}
