using Emm.Domain.Entities;

namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UnitOfMeasureSummaryResponse(
    Guid Id,
    string Code,
    string Name,
    string Symbol,
    string? Description,
    bool IsActive
);
