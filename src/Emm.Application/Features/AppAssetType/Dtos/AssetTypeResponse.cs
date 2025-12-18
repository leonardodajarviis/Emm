using Emm.Application.Features.AppParameterCatalog.Dtos;

namespace Emm.Application.Features.AppAssetType.Dtos;

public record AssetTypeResponse : AuditableEntityDtoResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public bool IsCodeGenerated {get; init;}
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public required Guid AssetCategoryId { get; init; }
    public IReadOnlyCollection<AssetParameterResponse> Parameters { get; init; } = [];
    public string? AssetCategoryName { get; init; }
}
