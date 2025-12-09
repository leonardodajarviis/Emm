# Value Objects

This document describes the Value Objects implemented in the Domain layer to eliminate primitive obsession and encapsulate business logic.

## Overview

Value Objects are immutable objects that represent descriptive aspects of the domain with no conceptual identity. They are defined by their attributes and implement equality based on those attributes.

## Implemented Value Objects

### 1. Money

**Location:** `src/Emm.Domain/ValueObjects/Money.cs`

**Purpose:** Represents a monetary amount with currency information.

**Features:**
- Immutable with value and currency
- Supports multiple currencies (VND, USD, EUR, JPY, CNY, KRW, THB, SGD)
- Validation: amount cannot be negative
- Mathematical operations: Add, Subtract, Multiply, Divide
- Operator overloads: +, -, *, /
- Comparison operators: ==, !=, <, >, <=, >=
- Cannot perform operations on different currencies

**Usage Examples:**
```csharp
// Creating Money
var price = new Money(100000, "VND");
var cost = Money.FromVND(50000);
var zero = Money.Zero("USD");

// Operations
var total = price + cost; // Both must be VND
var doubled = price * 2;
var half = price / 2;

// Comparisons
if (price > cost) { /* ... */ }
```

**Use Cases:**
- `MaintenanceTicket`: EstimatedCost, ActualCost
- `MaintenancePart`: UnitCost, TotalCost
- Any entity dealing with financial amounts

---

### 2. Email

**Location:** `src/Emm.Domain/ValueObjects/Email.cs`

**Purpose:** Represents a validated email address.

**Features:**
- Immutable email value
- Automatic normalization (lowercase, trimmed)
- RFC 5321 compliant (max 320 characters)
- Format validation using regex
- Additional validations:
  - No consecutive dots (..)
  - Cannot start or end with dot
- Helper properties: Domain, LocalPart
- Utility method: IsFromDomain()

**Usage Examples:**
```csharp
// Creating Email
var email = new Email("user@example.com");
var email2 = (Email)"admin@company.com"; // Explicit cast

// Properties
var domain = email.Domain;         // "example.com"
var localPart = email.LocalPart;   // "user"

// Validation
if (email.IsFromDomain("example.com")) { /* ... */ }

// Conversion
string emailString = email; // Implicit conversion to string
```

**Use Cases:**
- `User`: Email property
- Any entity requiring validated email addresses

---

### 3. Quantity

**Location:** `src/Emm.Domain/ValueObjects/Quantity.cs`

**Purpose:** Represents a quantity with unit of measure.

**Features:**
- Immutable value and unit symbol
- Automatic unit normalization (lowercase, trimmed)
- Validation: value cannot be negative, unit cannot be empty
- Mathematical operations: Add, Subtract, Multiply, Divide
- Operator overloads: +, -, *, /
- Comparison operators: ==, !=, <, >, <=, >=
- Cannot perform operations on different units
- Unit is stored as symbol (e.g., "kg", "l", "pcs") for display purposes
- Actual UnitOfMeasure validation should be done at application layer against DB

**Usage Examples:**
```csharp
// Creating Quantity
var weight = new Quantity(100, "kg");
var parts = Quantity.FromPieces(50);
var volume = Quantity.FromLiters(10);
var distance = Quantity.FromMeters(100);
var duration = Quantity.FromHours(8);

// Operations
var totalWeight = weight + new Quantity(50, "kg");
var doubled = weight * 2;
var half = weight / 2;

// Comparisons
if (weight > Quantity.FromKilograms(50)) { /* ... */ }

// Unit checking
if (weight.IsSameUnit(otherQuantity)) { /* ... */ }
```

**Use Cases:**
- `MaterialRequestLine`: RequestedQuantity, ApprovedQuantity, IssuedQuantity + Unit
- `MaintenancePart`: Quantity + Unit
- `AssetParameter`: Value + Unit (parameter readings)
- Any entity dealing with measurements

**Note on UnitOfMeasure Integration:**
- The `Quantity` Value Object stores the unit **symbol** (e.g., "kg") for lightweight operations
- The system has a separate `UnitOfMeasure` entity in the database for:
  - Managing available units with conversion factors
  - Unit type categorization (Length, Mass, Volume, etc.)
  - Unit conversion between compatible types
- At the **Application layer**, validate that the unit symbol exists in the `UnitOfMeasure` table
- For unit conversion operations, use a Domain Service with the `UnitOfMeasure` entity

---

## Design Principles

### 1. Immutability
All Value Objects are immutable. Once created, their state cannot be changed. Operations return new instances.

### 2. Equality
Value Objects implement value-based equality:
- `IEquatable<T>` interface
- Overridden `Equals()` and `GetHashCode()`
- Operator overloads (==, !=)

### 3. Validation
All validation happens in the constructor. Invalid states cannot be created.

### 4. Self-Validation
Value Objects contain their own validation logic, ensuring consistency across the domain.

### 5. No Side Effects
Operations on Value Objects do not modify the original object but return new instances.

---

## Benefits

1. **Type Safety**: Compile-time checking prevents passing wrong types
2. **Validation Centralization**: Validation logic in one place
3. **Expressiveness**: Code becomes more readable and self-documenting
4. **Domain Language**: Uses ubiquitous language from the domain
5. **Testability**: Easy to unit test in isolation

---

### 4. NaturalKey

**Location:** `src/Emm.Domain/ValueObjects/NaturalKey.cs`

**Purpose:** Represents a business natural key/code for entities.

**Features:**
- Immutable code value with optional prefix
- Automatic normalization (uppercase, trimmed)
- Validation: alphanumeric with dash (-), underscore (_), and dot (.)
- Max length: 50 characters
- Prefix validation support
- Factory methods for common entity types
- Custom pattern validation support
- Helper methods: Extract year, sequence number, remove prefix
- Tracks if code was auto-generated

**Usage Examples:**
```csharp
// Creating NaturalKey
var assetCode = NaturalKey.ForAsset("AST-2024-00001", isGenerated: true);
var empCode = NaturalKey.ForEmployee("EMP-12345");
var orgCode = NaturalKey.ForOrganization("ORG-IT-001");
var itemCode = NaturalKey.ForItem("ITM-BOLT-M10");
var paramCode = NaturalKey.ForParameter("PARAM-TEMP-01");

// Generic code without prefix
var code = NaturalKey.Create("CUSTOM-123");

// Custom pattern validation
var projectCode = NaturalKey.CreateWithPattern(
    "PRJ-2024-042",
    @"^PRJ-\d{4}-\d{3}$",
    "PRJ-YYYY-NNN");

// Helper methods
var year = assetCode.ExtractYear();           // 2024
var sequence = assetCode.ExtractSequenceNumber(); // 1
var withoutPrefix = assetCode.GetValueWithoutPrefix(); // "2024-00001"
var hasPrefix = assetCode.HasPrefix("AST");   // true

// Conversions
string codeString = assetCode;                // Implicit to string
var code2 = (NaturalKey)"CODE-123";          // Explicit from string
```

**Use Cases:**
- `Asset`: Code property
- `Employee`: Code property
- `Organization`: Code property
- `Item`: Code property
- `ParameterCatalog`: Code property
- Any entity with business-meaningful codes

**Design Notes:**
- Prefix is optional but enforced if provided
- `IsGenerated` flag tracks system-generated vs user-entered codes
- Factory methods provide type-safe prefix validation
- Custom patterns support flexible business rules

---

## Testing

All Value Objects have comprehensive unit tests in:
- `tests/Emm.Domain.Tests/ValueObjects/MoneyTests.cs` (39 tests)
- `tests/Emm.Domain.Tests/ValueObjects/EmailTests.cs` (30 tests)
- `tests/Emm.Domain.Tests/ValueObjects/QuantityTests.cs` (27 tests)
- `tests/Emm.Domain.Tests/ValueObjects/NaturalKeyTests.cs` (67 tests)

**Total: 133 tests**

Run tests:
```bash
dotnet test tests/Emm.Domain.Tests/Emm.Domain.Tests.csproj
```

---

## Future Value Objects (Priority 2)

### DateRange
**Purpose:** Represent start and end date pairs with validation
**Use Cases:** ScheduledStartDate/EndDate, ActualStartDate/EndDate

### PersonName
**Purpose:** Represent person names with FirstName, LastName, DisplayName
**Use Cases:** Employee name fields

### Address (Optional - Priority 3)
**Purpose:** Represent physical addresses
**Use Cases:** Location, Organization, Employee addresses

---

## Migration Guide

### Before (Primitive Obsession)
```csharp
public class MaintenanceTicket
{
    public decimal EstimatedCost { get; set; }
    public string Currency { get; set; }
}
```

### After (Using Value Object)
```csharp
public class MaintenanceTicket
{
    public Money EstimatedCost { get; private set; }
}
```

### EF Core Configuration
```csharp
builder.OwnsOne(t => t.EstimatedCost, money =>
{
    money.Property(m => m.Amount).HasColumnName("EstimatedCost");
    money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
});
```

---

## Best Practices

1. **Use Value Objects for**:
   - Money/currency amounts
   - Email addresses
   - Quantities with units
   - Date ranges
   - Codes with validation rules

2. **Don't Use Value Objects for**:
   - Simple primitives without business rules
   - Entity identifiers (use long/Guid)
   - Large complex objects (use Entities)

3. **Always**:
   - Keep them immutable
   - Validate in constructor
   - Implement equality
   - Write comprehensive tests

4. **Consider**:
   - Conversion operators for convenience
   - ToString() formatting
   - Factory methods for common cases
