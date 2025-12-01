using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record CreateParameterCatalogCommand(
    string Name,
    string? Description = null
) : IRequest<Result<object>>;
