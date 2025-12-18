namespace Emm.Application.Features.AppAssetModel.Commands;

public record ChangeThumbnailCommand(
    Guid AssetModelId,
    Guid FileId
) : IRequest<Result>;
