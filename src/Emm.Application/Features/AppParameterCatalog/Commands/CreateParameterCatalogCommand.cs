using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record CreateParameterCatalogCommand(
    string Name,
    Guid UnitOfMeasureId,
    string? Description = null
) : IRequest<Result<object>>;
