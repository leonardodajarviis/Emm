namespace Emm.Application.Features.AppAssetModel.Commands;

public record AddParametersToAssetModelCommand(long AssetModelId, List<long> ParameterIds) : IRequest<Result<object>>;