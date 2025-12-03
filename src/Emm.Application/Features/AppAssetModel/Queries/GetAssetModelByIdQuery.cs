using System.Text.Json.Serialization;
using Emm.Application.Authorization;
using Emm.Application.Features.AppAssetModel.Dtos;

namespace Emm.Application.Features.AppAssetModel.Queries;

public record GetAssetModelByIdQuery :
IRequest<Result<AssetModelDetailResponse>>,
IRequirePermission
{
    public long Id { get; set; }

    [JsonIgnore]
    public string[] RequiredPermissions => ["AssetModel.View"];

    public GetAssetModelByIdQuery(long id)
    {
        Id = id;
    }
}
