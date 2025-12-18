namespace Emm.Application.Features.AppAssetModel.Dtos;

public record UpdateAssetModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? AssetCategoryId { get; set; }
    public Guid? AssetTypeId { get; set; }
    public bool IsActive { get; set; } = true;
}