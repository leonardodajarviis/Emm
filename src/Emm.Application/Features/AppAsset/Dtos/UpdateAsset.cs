namespace Emm.Application.Features.AppAsset.Dtos;

public class UpdateAsset
{
    public required long Id { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }
}
