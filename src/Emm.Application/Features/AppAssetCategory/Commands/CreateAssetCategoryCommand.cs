namespace Emm.Application.Features.AppAssetCategory.Commands;

public record CreateAssetCategoryCommand(
    string Code,
    string Name,
    string? Description,
    bool IsCodeGenerated = false,
    bool IsActive = true
) : IRequest<Result<object>>;
