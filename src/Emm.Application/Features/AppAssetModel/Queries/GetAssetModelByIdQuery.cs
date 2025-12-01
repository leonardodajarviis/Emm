using Emm.Application.Features.AppAssetModel.Dtos;

namespace Emm.Application.Features.AppAssetModel.Queries;

public record GetAssetModelByIdQuery(
    long Id
) : IRequest<Result<AssetModelDetailResponse>>;