using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UnitOfMeasureResponse(
    Guid Id,
    string Code,
    string Name,
    string Symbol,
    string? Description,
    UnitType UnitType,
    string UnitTypeName,
    Guid? BaseUnitId,
    string? BaseUnitName,
    string? BaseUnitSymbol,
    decimal? ConversionFactor,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
