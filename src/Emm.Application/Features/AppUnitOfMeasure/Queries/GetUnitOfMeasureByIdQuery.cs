using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public record GetUnitOfMeasureByIdQuery(long Id) : IRequest<Result<object>>;
