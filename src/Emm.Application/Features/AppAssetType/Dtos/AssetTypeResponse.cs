using Emm.Application.Features.AppParameterCatalog.Dtos;

namespace Emm.Application.Features.AppAssetType.Dtos;

public record AssetTypeResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public required long AssetCategoryId { get; set; }
    public IReadOnlyCollection<AssetParameterResponse> Parameters { get; set; } = [];
    public string? AssetCategoryName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
