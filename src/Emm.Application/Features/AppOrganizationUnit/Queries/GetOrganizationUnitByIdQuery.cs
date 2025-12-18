namespace Emm.Application.Features.AppOrganizationUnit.Queries;

public record GetOrganizationUnitByIdQuery(Guid Id) : IRequest<Result<object>>;
