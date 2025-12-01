namespace Emm.Application.Features.AppAsset.Dtos;

public class CreateAsset
{
    public required string Code { get; set; }
    public required string DisplayName { get; set; }
    public required long AssetModelId { get; set; }
    public string? Description { get; set; }
}
