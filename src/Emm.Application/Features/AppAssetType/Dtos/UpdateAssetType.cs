namespace Emm.Application.Features.AppAssetType.Dtos;

public record UpdateAssetType
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Guid AssetCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<Guid> ParameterIds { get; set; } = [];
}
