namespace Emm.Application.Features.AppParameterCatalog.Dtos;

public record AssetParameterResponse
{
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string? Description { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public bool IsMaintenanceParameter { get; set; }
}
