namespace Emm.Application.Features.AppAssetCategory.Dtos;

public record AssetCategoryResponse
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Code {get; init;}
    public string? Description { get; init; }
    public required bool IsActive { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public string? UpdatedBy { get; init; }
    public long? CreatedByUserId { get; init; }
    public long? UpdatedByUserId { get; init; }
}
