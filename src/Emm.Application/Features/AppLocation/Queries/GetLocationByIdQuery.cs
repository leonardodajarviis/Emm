namespace Emm.Application.Features.AppLocation.Queries;

public record GetLocationByIdQuery(long Id) : IRequest<Result<object>>;
