using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record DeleteParameterCatalogCommand(Guid Id) : IRequest<Result<bool>>;
