# Unit Of Measure Seeder

## Mô tả
File này chứa dữ liệu seed cho các đơn vị đo lường cơ bản trong hệ thống.

## Các đơn vị đã được seed

### 1. Độ dài (Length)
- **Base Unit**: Mét (m)
- **Derived Units**:
  - Kilômét (km) - 1 km = 1000 m
  - Xentimét (cm) - 1 m = 100 cm
  - Milimét (mm) - 1 m = 1000 mm
  - Dặm (mi) - 1 mile = 1609.344 m
  - Thước (yd) - 1 yard = 0.9144 m
  - Feet (ft) - 1 feet = 0.3048 m
  - Inch (in) - 1 inch = 0.0254 m

### 2. Khối lượng (Mass)
- **Base Unit**: Kilôgam (kg)
- **Derived Units**:
  - Gram (g) - 1 kg = 1000 g
  - Miligram (mg) - 1 kg = 1,000,000 mg
  - Tấn (ton) - 1 ton = 1000 kg
  - Pound (lb) - 1 lb = 0.453592 kg
  - Ounce (oz) - 1 oz = 0.0283495 kg

### 3. Thể tích (Volume)
- **Base Unit**: Lít (L)
- **Derived Units**:
  - Mililít (mL) - 1 L = 1000 mL
  - Mét khối (m³) - 1 m³ = 1000 L
  - Gallon (gal) - 1 gal = 3.78541 L
  - Thùng dầu (bbl) - 1 barrel = 158.987 L

### 4. Thời gian (Time)
- **Base Unit**: Giây (s)
- **Derived Units**:
  - Phút (min) - 1 phút = 60 giây
  - Giờ (h) - 1 giờ = 3600 giây
  - Ngày (d) - 1 ngày = 86400 giây
  - Tuần (wk) - 1 tuần = 604800 giây
  - Tháng (mo) - 1 tháng ≈ 2592000 giây
  - Năm (yr) - 1 năm = 31536000 giây

### 5. Nhiệt độ (Temperature)
- Độ C (°C) - Celsius
- Độ F (°F) - Fahrenheit
- Kelvin (K) - Kelvin

> **Lưu ý**: Nhiệt độ không có conversion factor tuyến tính, mỗi đơn vị là base unit riêng.

### 6. Năng lượng (Energy)
- **Base Unit**: Joule (J)
- **Derived Units**:
  - Kilowatt giờ (kWh) - 1 kWh = 3,600,000 J
  - Watt giờ (Wh) - 1 Wh = 3600 J
  - Calorie (cal) - 1 cal = 4.184 J
  - Kilocalorie (kcal) - 1 kcal = 4184 J

### 7. Công suất (Power)
- **Base Unit**: Watt (W)
- **Derived Units**:
  - Kilowatt (kW) - 1 kW = 1000 W
  - Megawatt (MW) - 1 MW = 1,000,000 W
  - Mã lực (hp) - 1 hp = 745.7 W

### 8. Áp suất (Pressure)
- **Base Unit**: Pascal (Pa)
- **Derived Units**:
  - Bar (bar) - 1 bar = 100,000 Pa
  - Pound/inch² (psi) - 1 psi = 6894.76 Pa
  - Atmosphere (atm) - 1 atm = 101,325 Pa
  - Kilopascal (kPa) - 1 kPa = 1000 Pa
  - Megapascal (MPa) - 1 MPa = 1,000,000 Pa

### 9. Vận tốc (Speed)
- **Base Unit**: Mét/giây (m/s)
- **Derived Units**:
  - Kilômét/giờ (km/h) - 1 km/h = 0.277778 m/s
  - Dặm/giờ (mph) - 1 mph = 0.44704 m/s

### 10. Diện tích (Area)
- **Base Unit**: Mét vuông (m²)
- **Derived Units**:
  - Kilômét vuông (km²) - 1 km² = 1,000,000 m²
  - Xentimét vuông (cm²) - 1 m² = 10,000 cm²
  - Hecta (ha) - 1 ha = 10,000 m²
  - Mẫu (ac) - 1 acre = 4046.86 m²

### 11. Lực (Force)
- **Base Unit**: Newton (N)
- **Derived Units**:
  - Kilonewton (kN) - 1 kN = 1000 N
  - Kilôgam lực (kgf) - 1 kgf = 9.80665 N
  - Pound lực (lbf) - 1 lbf = 4.44822 N

### 12. Tần số (Frequency)
- **Base Unit**: Hertz (Hz)
- **Derived Units**:
  - Kilohertz (kHz) - 1 kHz = 1000 Hz
  - Megahertz (MHz) - 1 MHz = 1,000,000 Hz
  - Gigahertz (GHz) - 1 GHz = 1,000,000,000 Hz

### 13. Số lượng (Quantity)
- Cái (pc)
- Hộp (box)
- Thùng (ctn)
- Pallet (plt)
- Tá (dz) - 1 tá = 12 cái
- Gói (pk)
- Bộ (set)

### 14. Phần trăm (Percentage)
- Phần trăm (%)

### 15. Tiền tệ (Currency)
- Đồng Việt Nam (₫)
- Đô la Mỹ ($)
- Euro (€)

## Cách sử dụng

### 1. Tự động seed khi khởi động ứng dụng
Dữ liệu sẽ tự động được seed khi ứng dụng khởi động lần đầu tiên.

Trong file `Program.cs`:
```csharp
var app = builder.Build();

// Seed database with initial data
await app.SeedDatabaseAsync();
```

### 2. Seed thủ công (nếu cần)
Nếu muốn seed thủ công hoặc reset data:

```csharp
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<XDbContext>();
await UnitOfMeasureSeeder.SeedUnitOfMeasureDataAsync(context);
```

### 3. Kiểm tra
Seeder sẽ tự động kiểm tra xem dữ liệu đã tồn tại chưa trước khi seed:
```csharp
if (await context.Set<UnitOfMeasure>().AnyAsync())
    return; // Skip if data already exists
```

## Lưu ý quan trọng

### 1. BaseUnitId và ConversionFactor
- Các đơn vị **base** (cơ sở) sẽ có `BaseUnitId = null` và `ConversionFactor = null`
- Các đơn vị **derived** (dẫn xuất) phải có `ConversionFactor` để chuyển đổi về đơn vị cơ sở
- Do constructor của `UnitOfMeasure` không cho phép set `BaseUnitId`, nên hiện tại tất cả các đơn vị được tạo như base units

### 2. Cần cập nhật sau khi seed
Sau khi seed, bạn cần chạy migration hoặc script SQL để cập nhật `BaseUnitId` cho các derived units:

```sql
-- Example: Update BaseUnitId for Length derived units
UPDATE UnitOfMeasures SET BaseUnitId = (SELECT Id FROM UnitOfMeasures WHERE Code = 'M')
WHERE Code IN ('KM', 'CM', 'MM', 'MILE', 'YARD', 'FEET', 'INCH');

-- Mass
UPDATE UnitOfMeasures SET BaseUnitId = (SELECT Id FROM UnitOfMeasures WHERE Code = 'KG')
WHERE Code IN ('G', 'MG', 'TON', 'LB', 'OZ');

-- Volume
UPDATE UnitOfMeasures SET BaseUnitId = (SELECT Id FROM UnitOfMeasures WHERE Code = 'L')
WHERE Code IN ('ML', 'M3', 'GAL', 'BBL');

-- Tiếp tục cho các unit type khác...
```

### 3. Mở rộng thêm đơn vị
Để thêm đơn vị mới, thêm vào list `units` trong `SeedUnitOfMeasureDataAsync`:

```csharp
units.Add(new UnitOfMeasure(
    code: "NEW_CODE",
    name: "Tên đơn vị",
    symbol: "symbol",
    unitType: UnitType.Length,
    description: "Mô tả",
    baseUnitId: null,  // Sẽ cập nhật sau
    conversionFactor: 1.5m  // Hệ số chuyển đổi
));
```

## Migration

Sau khi chạy seeder, có thể cần tạo migration để update BaseUnitId:

```bash
cd src/Emm.Infrastructure
dotnet ef migrations add UpdateUnitOfMeasureBaseUnitIds
dotnet ef database update
```

Hoặc tạo script SQL để update sau khi seed xong.
