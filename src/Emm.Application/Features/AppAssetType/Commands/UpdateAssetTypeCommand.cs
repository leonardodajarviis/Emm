namespace Emm.Application.Features.AppAssetType.Commands;

public record UpdateAssetTypeCommand(
    Guid Id,
    UpdateAssetTypeBody Body
) : IRequest<Result>;

public record UpdateAssetTypeBody(
    string Name,
    Guid AssetCategoryId,
    string? Description,
    bool IsActive,
    IEnumerable<Guid> ParameterIds
);
