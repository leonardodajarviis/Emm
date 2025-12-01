namespace Emm.Application.Features.AppAssetModel.Dtos;

public record AssetModelSummaryResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? ThumbnailUrl { get; set; }
    public long? ParentId { get; set; }
    public string? ParentName { get; set; }
    public long? AssetCategoryId { get; set; }
    public string? AssetCategoryName { get; set; }
    public long? AssetTypeId { get;  set; }
    public string? AssetTypeName { get; set; }
    public bool IsActive { get;  set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get;  set; }
}
