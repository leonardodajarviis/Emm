namespace Emm.Application.Features.AppLocation.Queries;

public record GetLocationByIdQuery(Guid Id) : IRequest<Result<object>>;
