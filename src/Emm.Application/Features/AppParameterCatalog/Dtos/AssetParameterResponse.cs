namespace Emm.Application.Features.AppParameterCatalog.Dtos;

public record AssetParameterResponse
{
    public required long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public long UnitOfMeasureId { get; set; }
    public string? Description { get; set; }

    public string? UnitOfMeasureName { get; set; }
}
