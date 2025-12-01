namespace Emm.Application.Features.AppOrganizationUnitLevel.Queries;

public record GetOrganizationUnitLevelByIdQuery(long Id) : IRequest<Result<object>>;
