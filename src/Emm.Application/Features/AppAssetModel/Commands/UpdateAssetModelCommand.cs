namespace Emm.Application.Features.AppAssetModel.Commands;

public record UpdateAssetModelCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Notes,
    Guid? ParentId,
    Guid? AssetCategoryId,
    Guid? AssetTypeId,
    bool IsActive
) : IRequest<Result<object>>;