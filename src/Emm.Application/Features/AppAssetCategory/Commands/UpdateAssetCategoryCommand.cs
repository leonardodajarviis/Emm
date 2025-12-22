namespace Emm.Application.Features.AppAssetCategory.Commands;

public record UpdateAssetCategoryCommand(
    Guid Id,
    UpdateAssetCategoryBody Body
) : IRequest<Result>;

public record UpdateAssetCategoryBody(
    string Name,
    string? Description,
    bool IsActive
);
