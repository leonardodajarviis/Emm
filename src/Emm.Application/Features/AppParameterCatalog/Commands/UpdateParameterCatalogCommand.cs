using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record UpdateParameterCatalogCommand(
    Guid Id,
    string Name,
    string? Description = null
) : IRequest<Result<bool>>;
