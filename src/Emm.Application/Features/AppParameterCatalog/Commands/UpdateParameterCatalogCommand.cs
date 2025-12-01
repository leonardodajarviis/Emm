using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record UpdateParameterCatalogCommand(
    long Id,
    string Name,
    string? Description = null
) : IRequest<Result<bool>>;
