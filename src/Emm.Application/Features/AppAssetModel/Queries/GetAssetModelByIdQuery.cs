using Emm.Application.Features.AppAssetModel.Dtos;

namespace Emm.Application.Features.AppAssetModel.Queries;

public record GetAssetModelByIdQuery : IRequest<Result<AssetModelDetailResponse>>
{
    public Guid Id { get; set; }

    // [JsonIgnore]
    // public string[] RequiredPermissions => ["AssetModel.View"];

    public GetAssetModelByIdQuery(Guid id)
    {
        Id = id;
    }
}
