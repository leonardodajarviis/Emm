using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UpdateUnitOfMeasure(
    string Name,
    string Symbol,
    UnitType UnitType,
    string? Description = null,
    Guid? BaseUnitId = null,
    decimal? ConversionFactor = null
);
