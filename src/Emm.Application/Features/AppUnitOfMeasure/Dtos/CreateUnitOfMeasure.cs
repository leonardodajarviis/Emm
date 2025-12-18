using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record CreateUnitOfMeasure(
    string Name,
    string Symbol,
    UnitType UnitType,
    string? Description = null,
    Guid? BaseUnitId = null,
    decimal? ConversionFactor = null
);
