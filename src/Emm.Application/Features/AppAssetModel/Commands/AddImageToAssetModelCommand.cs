namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddImagesToAssetModelCommand(
    Guid AssetModelId,
    List<Guid> FileIds
) : IRequest<Result<object>>;
