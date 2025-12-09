using Emm.Domain.Entities.Inventory;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UnitOfMeasureSummaryResponse(
    long Id,
    string Code,
    string Name,
    string Symbol,
    string? Description,
    UnitType UnitType,
    string UnitTypeName,
    bool IsActive
);
