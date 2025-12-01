using Emm.Application.Common;
using Emm.Domain.Entities.Inventory;

namespace Emm.Application.Features.AppUnitOfMeasure.Queries;

public record GetUnitOfMeasuresByTypeQuery(UnitType UnitType) : IRequest<Result<object>>;
