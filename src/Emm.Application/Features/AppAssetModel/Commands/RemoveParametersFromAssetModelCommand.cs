namespace Emm.Application.Features.AppAssetModel.Commands;

public record RemoveParametersFromAssetModelCommand(long AssetModelId, List<long> ParameterIds) : IRequest<Result<object>>;
