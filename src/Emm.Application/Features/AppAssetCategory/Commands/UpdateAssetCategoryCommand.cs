namespace Emm.Application.Features.AppAssetCategory.Commands;

public record UpdateAssetCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<Result>;
