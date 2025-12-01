using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Commands;

public record DeleteParameterCatalogCommand(long Id) : IRequest<Result<bool>>;
