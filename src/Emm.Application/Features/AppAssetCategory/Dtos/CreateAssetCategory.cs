namespace Emm.Application.Features.AppAssetCategory.Dtos;

public class CreateAssetCategory
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
