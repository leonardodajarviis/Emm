using Emm.Domain.Entities.Inventory;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UnitOfMeasureResponse(
    long Id,
    string Code,
    string Name,
    string Symbol,
    string? Description,
    UnitType UnitType,
    string UnitTypeName,
    long? BaseUnitId,
    string? BaseUnitName,
    string? BaseUnitSymbol,
    decimal? ConversionFactor,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
