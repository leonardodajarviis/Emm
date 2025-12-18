using Emm.Application.Common;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public record GetUnitOfMeasureByIdQuery(Guid Id) : IRequest<Result<object>>;
