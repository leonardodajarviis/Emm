namespace Emm.Application.Features.AppOrganizationUnitLevel.Queries;

public record GetOrganizationUnitLevelByIdQuery(Guid Id) : IRequest<Result<object>>;
