using Emm.Domain.Entities.Inventory;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UnitOfMeasureListResponse(
    long Id,
    string Code,
    string Name,
    string Symbol,
    UnitType UnitType,
    string UnitTypeName,
    bool IsActive
);
