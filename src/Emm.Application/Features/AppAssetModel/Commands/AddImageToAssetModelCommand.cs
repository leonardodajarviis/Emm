namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddImagesToAssetModelCommand(
    long AssetModelId,
    List<Guid> FileIds
) : IRequest<Result<object>>;
