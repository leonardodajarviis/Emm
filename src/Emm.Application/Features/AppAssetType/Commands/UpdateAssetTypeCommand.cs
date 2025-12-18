namespace Emm.Application.Features.AppAssetType.Commands;

public record UpdateAssetTypeCommand(
    Guid Id,
    string Name,
    Guid AssetCategoryId,
    string? Description,
    bool IsActive,
    IEnumerable<Guid> ParameterIds
) : IRequest<Result<object>>;
