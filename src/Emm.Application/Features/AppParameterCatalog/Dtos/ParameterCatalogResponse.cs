namespace Emm.Application.Features.AppParameterCatalog.Dtos;

public record ParameterCatalogResponse(
    long Id,
    string Code,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
