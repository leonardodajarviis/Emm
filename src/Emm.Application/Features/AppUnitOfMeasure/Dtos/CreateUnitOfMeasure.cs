using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record CreateUnitOfMeasure(
    string Name,
    string Symbol,
    UnitType UnitType,
    string? Description = null,
    long? BaseUnitId = null,
    decimal? ConversionFactor = null
);
