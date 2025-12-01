namespace Emm.Application.Features.AppParameterCatalog.Dtos;

public record UpdateParameterCatalog(
    string Name,
    string? Description = null
);
