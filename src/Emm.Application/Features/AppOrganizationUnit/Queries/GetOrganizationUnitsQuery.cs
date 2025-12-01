namespace Emm.Application.Features.AppOrganizationUnit.Queries;

public record GetOrganizationUnitsQuery(QueryParam QueryRequest) : IRequest<Result<PagedResult>>;
