using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities;

public class UnitOfMeasure : AggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Symbol { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? BaseUnitId { get; private set; }
    public decimal? ConversionFactor { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsLooked { get; private set; }
    private readonly List<UnitOfMeasure> _derivedUnits;
    public IReadOnlyCollection<UnitOfMeasure> DerivedUnits => _derivedUnits.AsReadOnly();

    public UnitOfMeasure(
        string code,
        string name,
        string symbol,
        string? description = null,
        Guid? baseUnitId = null,
        decimal? conversionFactor = null)
    {
        _derivedUnits = [];
        Code = code;
        Name = name;
        Symbol = symbol;
        Description = description;
        BaseUnitId = baseUnitId;
        ConversionFactor = conversionFactor;
        IsActive = true;
    }

    public void Update(
        string name,
        string symbol,
        string? description = null,
        Guid? baseUnitId = null,
        decimal? conversionFactor = null)
    {
        Name = name;
        Symbol = symbol;
        Description = description;
        BaseUnitId = baseUnitId;
        ConversionFactor = conversionFactor;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }



    private UnitOfMeasure()
    {
        _derivedUnits = [];
    } // EF Core constructor
}
