namespace Emm.Application.Features.AppAssetCategory.Dtos;

public class AssetCategoryResponse
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required string Code {get; set;}
    public string? Description { get; set; }
    public required bool IsActive { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
