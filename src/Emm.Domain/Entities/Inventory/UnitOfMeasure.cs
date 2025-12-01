using Emm.Domain.Abstractions;

namespace Emm.Domain.Entities.Inventory;

public class UnitOfMeasure : AggregateRoot
{
    public long Id { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Symbol { get; private set; } = null!;
    public string? Description { get; private set; }
    public UnitType UnitType { get; private set; }
    public long? BaseUnitId { get; private set; }
    public decimal? ConversionFactor { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation properties
    public UnitOfMeasure? BaseUnit { get; private set; }
    public ICollection<UnitOfMeasure> DerivedUnits { get; private set; } = new List<UnitOfMeasure>();

    public UnitOfMeasure(
        string code,
        string name,
        string symbol,
        UnitType unitType,
        string? description = null,
        long? baseUnitId = null,
        decimal? conversionFactor = null)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        UnitType = unitType;
        Description = description;
        BaseUnitId = baseUnitId;
        ConversionFactor = conversionFactor;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        ValidateConversionFactor();
    }

    public void Update(
        string name,
        string symbol,
        UnitType unitType,
        string? description = null,
        long? baseUnitId = null,
        decimal? conversionFactor = null)
    {
        Name = name;
        Symbol = symbol;
        UnitType = unitType;
        Description = description;
        BaseUnitId = baseUnitId;
        ConversionFactor = conversionFactor;
        UpdatedAt = DateTime.UtcNow;

        ValidateConversionFactor();
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal ConvertTo(decimal value, UnitOfMeasure targetUnit)
    {
        if (UnitType != targetUnit.UnitType)
        {
            throw new InvalidOperationException($"Cannot convert between different unit types: {UnitType} and {targetUnit.UnitType}");
        }

        // If converting to the same unit
        if (Id == targetUnit.Id)
        {
            return value;
        }

        // Convert to base unit first
        decimal valueInBaseUnit = ConversionFactor.HasValue
            ? value * ConversionFactor.Value
            : value;

        // Convert from base unit to target unit
        decimal result = targetUnit.ConversionFactor.HasValue
            ? valueInBaseUnit / targetUnit.ConversionFactor.Value
            : valueInBaseUnit;

        return result;
    }

    private void ValidateConversionFactor()
    {
        if (BaseUnitId.HasValue && !ConversionFactor.HasValue)
        {
            throw new InvalidOperationException("Conversion factor is required when base unit is specified");
        }

        if (!BaseUnitId.HasValue && ConversionFactor.HasValue)
        {
            throw new InvalidOperationException("Base unit is required when conversion factor is specified");
        }

        if (ConversionFactor.HasValue && ConversionFactor.Value <= 0)
        {
            throw new InvalidOperationException("Conversion factor must be greater than zero");
        }
    }

    private UnitOfMeasure() { } // EF Core constructor
}

public enum UnitType
{
    Length = 1,          // km, m, cm, mm, mile, yard, feet, inch
    Mass = 2,            // kg, g, mg, ton, pound, ounce
    Volume = 3,          // liter, ml, gallon, barrel
    Time = 4,            // second, minute, hour, day, week, month, year
    Temperature = 5,     // Celsius, Fahrenheit, Kelvin
    Energy = 6,          // kWh, Wh, Joule, calorie
    Power = 7,           // kW, W, HP
    Pressure = 8,        // bar, psi, pascal, atm
    Speed = 9,           // km/h, m/s, mph
    Area = 10,           // m², km², acre, hectare
    Force = 11,          // Newton, kgf, lbf
    Frequency = 12,      // Hz, kHz, MHz
    Quantity = 13,       // piece, box, pallet, unit
    Percentage = 14,     // %
    Currency = 15,       // VND, USD, EUR
    Other = 99           // Other custom units
}
