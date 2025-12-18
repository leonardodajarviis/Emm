namespace Emm.Application.Features.AppAssetType.Commands;

public record CreateAssetTypeCommand(
    bool IsCodeGenerated,
    string Code,
    string Name,
    string? Description,
    Guid AssetCategoryId,
    Guid[] ParameterIds,
    bool IsActive = true
) : IRequest<Result<object>>;
