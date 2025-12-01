namespace Emm.Application.Features.AppAssetType.Commands;

public record UpdateAssetTypeCommand(
    long Id,
    string Name,
    long AssetCategoryId,
    string? Description,
    bool IsActive
) : IRequest<Result<object>>;
