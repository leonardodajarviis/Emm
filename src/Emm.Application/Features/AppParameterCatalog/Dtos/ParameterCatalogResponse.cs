namespace Emm.Application.Features.AppParameterCatalog.Dtos;

public record ParameterCatalogResponse(
    Guid Id,
    string Code,
    string Name,
    string? UnitOfMeasureName,
    string? Description,
    DateTime CreatedAt,
    DateTime? ModifiedAt
);
