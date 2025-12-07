namespace Emm.Application.Features.AppAssetType.Commands;

public record UpdateAssetTypeCommand(
    long Id,
    string Name,
    long AssetCategoryId,
    string? Description,
    bool IsActive,
    IEnumerable<long> ParameterIds
) : IRequest<Result<object>>;
