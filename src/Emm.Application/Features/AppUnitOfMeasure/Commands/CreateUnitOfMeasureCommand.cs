namespace Emm.Application.Features.AppUnitOfMeasure.Commands;

public record CreateUnitOfMeasureCommand(
    string Name,
    string Symbol,
    string? Description = null,
    Guid? BaseUnitId = null,
    decimal? ConversionFactor = null
) : IRequest<Result<object>>;
