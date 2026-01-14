namespace Emm.Application.Features.AppItem.Dtos;

public record ItemSummaryDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; } = null!;

    public IReadOnlyCollection<UnitOfMeasureCategoryLineResponseDto> UnitOfMeasureCategoryLines { get; set; } = [];
}

public record UnitOfMeasureCategoryLineResponseDto
{
    public Guid UnitOfMeasureId {get; set;}
    public string UnitOfMeasureCode { get; set; } = null!;
    public string UnitOfMeasureName { get; set; } = null!;
}
