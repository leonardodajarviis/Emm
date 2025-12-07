namespace Emm.Application.Features.AppAssetCategory.Commands;

public record UpdateAssetCategoryCommand(
    long Id,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<Result>;
