using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public record GetUnitOfMeasuresQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
