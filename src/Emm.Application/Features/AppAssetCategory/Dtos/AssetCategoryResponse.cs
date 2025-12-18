namespace Emm.Application.Features.AppAssetCategory.Dtos;

public record AssetCategoryResponse: AuditableEntityDtoResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code {get; init;}
    public string? Description { get; init; }
    public bool IsCodeGenerated { get; init; }
    public required bool IsActive { get; init; }
}
