using Emm.Application.Common;
using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record UpdateUnitOfMeasureCommand(
    Guid Id,
    string Name,
    string Symbol,
    UnitType UnitType,
    string? Description = null,
    Guid? BaseUnitId = null,
    decimal? ConversionFactor = null
) : IRequest<Result<object>>;
