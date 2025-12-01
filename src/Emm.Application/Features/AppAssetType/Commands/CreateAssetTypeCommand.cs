namespace Emm.Application.Features.AppAssetType.Commands;

public record CreateAssetTypeCommand(
    string Name,
    string? Description,
    long AssetCategoryId,
    long[] ParameterIds,
    bool IsActive = true 
) : IRequest<Result<object>>;
