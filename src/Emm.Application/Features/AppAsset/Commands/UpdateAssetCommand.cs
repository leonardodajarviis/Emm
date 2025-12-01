namespace Emm.Application.Features.AppAsset.Commands;

public record UpdateAssetCommand(
    long Id,
    string DisplayName,
    string? Description
) : IRequest<Result<object>>;
