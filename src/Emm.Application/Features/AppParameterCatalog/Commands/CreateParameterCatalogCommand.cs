using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record CreateParameterCatalogCommand(
    string Name,
    long UnitOfMeasureId,
    string? Description = null
) : IRequest<Result<object>>;
