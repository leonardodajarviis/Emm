namespace Emm.Application.Features.AppAsset.Commands;

public record UpdateAssetCommand(
    Guid Id,
    string DisplayName,
    string? Description
) : IRequest<Result<object>>;
