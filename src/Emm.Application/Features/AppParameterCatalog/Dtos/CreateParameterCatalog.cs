namespace Emm.Application.Features.AppParameterCatalog.Dtos;

public record CreateParameterCatalog(
    string Name,
    string? Description = null
);
