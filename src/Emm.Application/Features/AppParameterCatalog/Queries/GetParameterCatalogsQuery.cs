using Emm.Application.Features.AppParameterCatalog.Dtos;
using Emm.Application.Common;

namespace Emm.Application.Features.AppParameterCatalog.Queries;

public record GetParameterCatalogsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
