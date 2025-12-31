namespace Emm.Application.Features.AppAsset.Commands
{
    public record CreateAssetCommand(
        bool IsCodeGenerated,
        string? Code,
        string DisplayName,
        Guid AssetModelId,
        string? Description
    ) : IRequest<Result<object>>;
}
