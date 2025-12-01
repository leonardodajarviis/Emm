namespace Emm.Application.Features.AppAssetCategory.Commands;

public record CreateAssetCategoryCommand(
    string Name,
    string? Description,
    bool IsActive = true
) : IRequest<Result<object>>;
