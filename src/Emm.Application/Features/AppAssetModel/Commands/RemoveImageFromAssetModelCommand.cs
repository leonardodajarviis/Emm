namespace Emm.Application.Features.AppAssetModel.Commands;

public record RemoveImagesFromAssetModelCommand(
    long AssetModelId,
    List<Guid> FileIds
) : IRequest<Result<object>>;
