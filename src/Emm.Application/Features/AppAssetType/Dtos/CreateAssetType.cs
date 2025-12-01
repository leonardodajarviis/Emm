namespace Emm.Application.Features.AppAssetType.Dtos;

public class CreateAssetType
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required long AssetCategoryId { get; set; }
    public long [] ParameterIds { get; set; } = [];
    public bool IsActive { get; set; } = true;
}
