namespace Emm.Application.Features.AppAssetModel.Dtos;

public record UpdateAssetModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public long? ParentId { get; set; }
    public long? AssetCategoryId { get; set; }
    public long? AssetTypeId { get; set; }
    public bool IsActive { get; set; } = true;
}