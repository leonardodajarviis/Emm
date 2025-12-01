namespace Emm.Application.Features.AppAssetCategory.Dtos;

public class UpdateAssetCategory
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsActive { get; set; }
}
