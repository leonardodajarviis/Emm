namespace Emm.Application.Features.AppAsset.Commands
{
    public record CreateAssetCommand(
        string Code,
        string DisplayName,
        Guid AssetModelId,
        string? Description
    ) : IRequest<Result<object>>;
}
