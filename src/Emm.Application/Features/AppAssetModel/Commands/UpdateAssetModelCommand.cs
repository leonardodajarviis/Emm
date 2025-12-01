namespace Emm.Application.Features.AppAssetModel.Commands;

public record UpdateAssetModelCommand(
    long Id,
    string Name,
    string? Description,
    string? Notes,
    long? ParentId,
    long? AssetCategoryId,
    long? AssetTypeId,
    bool IsActive
) : IRequest<Result<object>>;