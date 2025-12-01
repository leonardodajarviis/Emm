using Emm.Application.Features.AppAssetType.Dtos;

namespace Emm.Application.Features.AppAssetType.Queries;

public record GetAssetTypeByIdQuery(
    long Id
) : IRequest<Result<AssetTypeResponse>>;
