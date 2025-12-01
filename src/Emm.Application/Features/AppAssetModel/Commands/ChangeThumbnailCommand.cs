namespace Emm.Application.Features.AppAssetModel.Commands;

public record ChangeThumbnailCommand(
    long AssetModelId,
    Guid FileId
) : IRequest<Result>;
