namespace Emm.Application.Features.AppAssetModel.Commands;

public record RemoveImagesFromAssetModelCommand(
    Guid AssetModelId,
    List<Guid> FileIds
) : IRequest<Result<object>>;
