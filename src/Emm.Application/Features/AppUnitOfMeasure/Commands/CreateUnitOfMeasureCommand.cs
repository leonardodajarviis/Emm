using Emm.Application.Common;
using Emm.Domain.Entities.Inventory;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record CreateUnitOfMeasureCommand(
    string Name,
    string Symbol,
    UnitType UnitType,
    string? Description = null,
    long? BaseUnitId = null,
    decimal? ConversionFactor = null
) : IRequest<Result<object>>;
