namespace Emm.Application.Features.AppOrganizationUnit.Queries;

public record GetOrganizationUnitByIdQuery(long Id) : IRequest<Result<object>>;
