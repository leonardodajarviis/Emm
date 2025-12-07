namespace Emm.Application.Features.AppAssetType.Dtos;

public record UpdateAssetType
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long AssetCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<long> ParameterIds { get; set; } = [];
}
