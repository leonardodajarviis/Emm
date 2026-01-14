namespace Emm.Application.Features.AppUnitOfMeasure.Dtos;

public record UpdateUnitOfMeasure(
    string Name,
    string Symbol,
    string? Description = null,
    Guid? BaseUnitId = null,
    decimal? ConversionFactor = null
);
