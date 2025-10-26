# Mini E-Commerce - Strategy & Decorator Pattern

## 📋 Giới thiệu

Dự án **Mini E-Commerce** là một hệ thống thương mại điện tử đơn giản với **2 Design Patterns chính**:
1. **Strategy Pattern** - Tính phí vận chuyển linh hoạt theo chiến lược
2. **Decorator Pattern** - Thêm các tính năng bổ sung (bảo hiểm, gói quà, v.v.)

Người dùng có thể duyệt sản phẩm, thêm vào giỏ hàng, chọn phương thức vận chuyển, và tùy chỉnh thêm các dịch vụ bổ sung.

## 🎯 Mục tiêu

- ✅ Áp dụng **Strategy Pattern** cho tính phí vận chuyển cơ bản
- ✅ Áp dụng **Decorator Pattern** cho các tính năng bổ sung
- ✅ Clean Architecture - Tách biệt rõ ràng các layer
- ✅ Dễ dàng mở rộng mà không thay đổi code hiện có

## 🏗️ Cấu trúc dự án

```
DACK/
├── README.md                        # ← Tài liệu chính (bạn đang đọc)
│
├── 1_Source/                        # 📦 Mã nguồn chương trình
│   ├── .gitignore
│   ├── MiniECommerce.sln
│   ├── src/
│   │   ├── MiniECommerce.Core/      # Domain layer
│   │   │   ├── Entities/            # User, Product, Order, CartItem
│   │   │   ├── Interfaces/          # IShippingStrategy
│   │   │   ├── Models/              # OrderContext
│   │   │   ├── Strategies/          # 4 shipping strategies
│   │   │   └── Decorators/          # 6 shipping decorators (NEW!)
│   │   ├── MiniECommerce.Infrastructure/
│   │   │   └── Data/                # DbContext, DbInitializer
│   │   └── MiniECommerce.API/       # Application layer
│   │       ├── Controllers/         # REST API endpoints
│   │       ├── Services/            # Business logic
│   │       └── DTOs/                # Data transfer objects
│   └── tests/
│       └── MiniECommerce.Tests/     # 26 unit tests
│
├── 2_Executable/                    # 🚀 File chạy trực tiếp
│   ├── run.ps1                      # Windows script
│   ├── run.sh                       # Linux/macOS script
│   └── MiniECommerce/               # Compiled application
│
└── 3_Database/                      # 💾 Database & SQL scripts
    ├── ecommerce.db                 # SQLite database
    ├── schema.sql                   # Database schema
    ├── seed_data.sql                # Sample data
    └── reset_database.sql           # Reset script
```

## 🎨 Design Patterns

### 1. Strategy Pattern - Phương thức vận chuyển

**4 Strategies:**
```
┌─────────────────────────┐
│  IShippingStrategy      │
│  (Interface)            │
├─────────────────────────┤
│ + Calculate(context)    │
│ + Name                  │
│ + GetCalculationDetails │
└───────────┬─────────────┘
            │
    ┌───────┴───────────────────────────────┐
    │                                       │
┌───▼──────────────┐              ┌────────▼──────────┐
│ StandardStrategy │              │ ExpressStrategy   │
│ (3-5 days)       │              │ (1-2 days)        │
└──────────────────┘              └───────────────────┘
    
┌──────────────────┐              ┌───────────────────┐
│ SameDayStrategy  │              │ EcoStrategy       │
│ (cutoff 2PM)     │              │ (eco-friendly)    │
└──────────────────┘              └───────────────────┘
```

### 2. Decorator Pattern - Tính năng bổ sung (NEW!)

**6 Decorators có thể kết hợp:**
```
┌─────────────────────────────┐
│  ShippingDecorator          │
│  (Abstract Base)            │
│  wraps IShippingStrategy    │
└──────────┬──────────────────┘
           │
    ┌──────┴────────────────────────────────────┐
    │                                           │
┌───▼─────────────────┐           ┌─────────────▼──────────┐
│ InsuranceDecorator  │           │ GiftWrappingDecorator  │
│ + ~2% order value   │           │ + $3.50/item + $1.50   │
└─────────────────────┘           └────────────────────────┘

┌─────────────────────┐           ┌────────────────────────┐
│ SignatureRequired   │           │ PriorityHandling       │
│ + $2.50-$4.00       │           │ + $5.00 + $0.50/kg     │
└─────────────────────┘           └────────────────────────┘

┌─────────────────────┐           ┌────────────────────────┐
│ WeekendDelivery     │           │ ... có thể thêm nữa    │
│ + $6-$10            │           │                        │
└─────────────────────┘           └────────────────────────┘
```

**Ví dụ kết hợp:**
```
Express Strategy ($15.00)
  → + Insurance ($2.40)
  → + Gift Wrapping ($5.00)
  → + Signature Required ($2.50)
  → + Weekend Delivery ($8.00)
= Total: $32.90
```

## 🚀 Chức năng

### User (Người dùng)
1. **Đăng ký/Đăng nhập** - JWT Authentication
2. **Danh sách & Chi tiết sản phẩm** - Tìm kiếm, lọc theo danh mục
3. **Giỏ hàng** - Thêm/Xóa/Cập nhật số lượng, xem subtotal
4. **Chọn phương thức vận chuyển** - Hiển thị phí ước tính theo Strategy
5. **Checkout** - Breakdown chi tiết (subtotal, thuế, khuyến mãi, phí ship, tổng)
6. **Lịch sử đơn hàng** - Xem chi tiết phí vận chuyển

### Admin (Quản trị viên)
1. **CRUD Sản phẩm** - Quản lý giá, tồn kho, trọng lượng
2. **Cấu hình tham số vận chuyển** - Chỉnh sửa paramsJSON cho từng chiến lược
3. **Báo cáo** - Số đơn theo ngày, tổng phí vận chuyển

## 📊 Strategy Pattern - Chi tiết các chiến lược

### 1. Standard Shipping (Giao hàng tiêu chuẩn)
**Công thức:** `base + perKg × weight × regionFactor`

**Tham số mặc định:**
```json
{
  "BaseFee": 20000,
  "PerKgFee": 5000,
  "RegionFactors": {
    "North": 1.0,
    "Central": 1.2,
    "South": 1.5
  }
}
```

**Ví dụ:**
- Trọng lượng: 2kg
- Vùng: South
- Phí = (20,000 + 5,000 × 2) × 1.5 = **45,000 VND**

### 2. Express Shipping (Giao hàng nhanh)
**Công thức:** `base × 1.2 + perKg × weight + surgeByTime`

**Tham số mặc định:**
```json
{
  "BaseFee": 30000,
  "BaseMultiplier": 1.2,
  "PerKgFee": 8000,
  "PeakHourSurge": 15000
}
```

**Ví dụ (Giờ cao điểm 7-9AM, 5-7PM):**
- Trọng lượng: 2kg
- Giờ đặt: 8:00 AM
- Phí = 30,000 × 1.2 + 8,000 × 2 + 15,000 = **67,000 VND**

### 3. Same-Day Shipping (Giao trong ngày)
**Công thức:** `baseHigh + perKm × distance`

**Ràng buộc:** Chỉ áp dụng trước 2:00 PM

**Tham số mặc định:**
```json
{
  "BaseFee": 50000,
  "PerKmFee": 3000,
  "CutoffHour": 14
}
```

**Ví dụ:**
- Khoảng cách: 10km
- Giờ đặt: 1:00 PM
- Phí = 50,000 + 3,000 × 10 = **80,000 VND**

### 4. Eco Shipping (Giao hàng tiết kiệm)
**Công thức:** `(base + perKg × weight) × (1 - bulkDiscount)` nếu weight ≥ threshold

**Tham số mặc định:**
```json
{
  "BaseFee": 15000,
  "PerKgFee": 3000,
  "BulkWeightThreshold": 10.0,
  "BulkDiscount": 0.15
}
```

**Ví dụ (Đơn bulk):**
- Trọng lượng: 12kg (≥ 10kg)
- Phí = (15,000 + 3,000 × 12) × (1 - 0.15) = **43,350 VND**

## 💰 Công thức tính tổng đơn hàng

```
subtotal      = Σ(qty × unitPrice)
discount      = min(maxDiscount, subtotal × discountRate) hoặc valueFixed
tax           = subtotal × TAX_RATE (10%)
shippingFee   = ShippingStrategy.Calculate(context)
grandTotal    = subtotal - discount + tax + shippingFee
```

**Lưu ý:**
- Thuế cố định: 10% (TAX_RATE)
- Khuyến mãi đơn giản: SAVE10 (10%), SAVE50K (50,000 VND), VIP20 (20%)
- Max discount: 500,000 VND

## � Cách sử dụng nhanh

### Cách 1: Chạy từ Executable (Khuyến nghị cho người dùng)

```powershell
# Chỉ cần .NET 8.0 Runtime (không cần SDK)
cd 2_Executable
.\run.ps1

# Hoặc trên Linux/macOS:
./run.sh
```

Ứng dụng sẽ chạy tại: **http://localhost:5000**  
Swagger UI: **http://localhost:5000/swagger**

### Cách 2: Build & Run từ Source Code (Cho developers)

**Yêu cầu:**
- .NET 8.0 SDK
- Visual Studio 2022 / VS Code / Rider

```powershell
cd 1_Source

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run API
cd src\MiniECommerce.API
dotnet run
```

API sẽ chạy tại: `https://localhost:5001`

### Test tự động

```powershell
cd 1_Source
dotnet test --verbosity normal

# Kết quả: 26/26 tests passed ✅
```

## 📡 API Endpoints

### Public APIs

#### Authentication
```http
POST /api/auth/register
POST /api/auth/login
```

#### Products
```http
GET  /api/products?search=laptop&category=Electronics
GET  /api/products/{id}
```

#### Cart (Requires Auth)
```http
GET    /api/cart
POST   /api/cart/items
PATCH  /api/cart/items/{id}
DELETE /api/cart/items/{id}
DELETE /api/cart
```

#### Shipping (Requires Auth)

**1. Tính phí vận chuyển cơ bản (Strategy Pattern):**
```http
POST /api/shipping/options
```

**Request:**
```json
{
  "weight": 5.0,
  "distance": 15.0,
  "region": "North"
}
```

**Response:** 4 strategies với phí ước tính

**2. Tính phí với các tính năng bổ sung (Decorator Pattern - NEW!):**
```http
POST /api/shipping/options-with-addons
```

**Request:**
```json
{
  "methodCode": "EXPRESS",
  "weight": 2.5,
  "distance": 10.0,
  "region": "North",
  "addInsurance": true,
  "addGiftWrapping": true,
  "requireSignature": true,
  "requireAdultSignature": false,
  "addPriorityHandling": true,
  "weekendDelivery": "Saturday"
}
```

**Response:**
```json
{
  "baseFee": 15.00,
  "totalFee": 32.65,
  "appliedAddons": [
    "Insurance ($2.40)",
    "Gift Wrapping ($5.00)",
    "Signature Required ($2.50)",
    "Priority Handling ($6.25)",
    "Weekend Delivery - Saturday ($8.00)"
  ],
  "calculationDetails": "Express (base: $15.00) + Insurance ($2.40) + Gift Wrap ($5.00) + Signature ($2.50) + Priority ($6.25) + Weekend ($8.00) = $32.65"
}
```

**3. Demo Decorator Pattern (Educational):**
```http
POST /api/shipping/decorator-demo
```

Hiển thị 5 ví dụ về cách decorators hoạt động

#### Orders (Requires Auth)
```http
POST /api/orders/checkout
GET  /api/orders
GET  /api/orders/{id}
```

**Checkout Request:**
```json
{
  "shippingMethodCode": "STANDARD",
  "region": "North",
  "distance": 10.0,
  "discountCode": "SAVE10"
}
```

**Checkout Response:**
```json
{
  "orderId": 1,
  "subtotal": 1000000,
  "discount": 100000,
  "tax": 100000,
  "taxRate": 0.1,
  "shippingFee": 45000,
  "shippingMethodCode": "STANDARD",
  "shippingCalculationDetails": "Standard Shipping: Base(20,000) + PerKg(5,000) × Weight(5.00kg) × RegionFactor[North](1.00) = 45,000 VND",
  "grandTotal": 1045000,
  "status": "Pending"
}
```

### Admin APIs (Requires Admin Role)

```http
POST   /api/products
PUT    /api/products/{id}
DELETE /api/products/{id}

GET    /api/shipping/methods
PUT    /api/shipping/methods/{id}

GET    /api/admin/reports/daily?date=2024-10-24
```

## 🧪 Testing

### Unit Tests

**Tất cả tests nằm trong:** `1_Source/tests/MiniECommerce.Tests/`

✅ **Strategy Pattern Tests:**
- Boundary tests (weight = 0, cutoff time, etc.)
- Formula validation cho 4 strategies
- Region factors, peak hours
- Custom params

✅ **Integration Tests:**
- End-to-end shipping calculation
- Multiple strategies comparison

### Test Coverage

```
Total:     26 tests
Passed:    26 tests ✅
Failed:    0 tests
Duration:  ~1 second
```

**Chạy tests:**
```powershell
cd 1_Source
dotnet test

# Với chi tiết:
dotnet test --verbosity detailed
```

**Test các Decorators:**
- Insurance calculation (2% of order value)
- Gift wrapping per item
- Signature requirements
- Priority handling by weight
- Weekend delivery premium
- Multiple decorators stacking

## 📚 Demo Scenario (3-5 phút)

### Bước 1: Đăng ký & Đăng nhập
```http
POST /api/auth/register
{
  "email": "demo@example.com",
  "password": "Demo123!"
}
```

### Bước 2: Duyệt sản phẩm & Thêm giỏ hàng
```http
GET /api/products

POST /api/cart/items
{
  "productId": 1,  // Laptop Dell XPS
  "quantity": 1
}

POST /api/cart/items
{
  "productId": 4,  // Nike Air Max
  "quantity": 2
}
```

### Bước 3: Xem các tùy chọn vận chuyển
```http
POST /api/shipping/options
{
  "weight": 0,      // Sẽ lấy từ giỏ hàng
  "distance": 15.0,
  "region": "South"
}
```

**Response hiển thị 4 phương thức:**
- Standard: 63,000 VND
- Express: 88,000 VND
- Same-Day: 95,000 VND
- Eco: 36,000 VND

### Bước 4: Checkout với Strategy đã chọn
```http
POST /api/orders/checkout
{
  "shippingMethodCode": "EXPRESS",
  "region": "South",
  "distance": 15.0,
  "discountCode": "VIP20"
}
```

**Response breakdown:**
```json
{
  "orderId": 1,
  "subtotal": 32000000,
  "discount": 500000,    // Max discount
  "tax": 3200000,        // 10%
  "shippingFee": 88000,  // Express strategy
  "grandTotal": 34788000,
  "shippingCalculationDetails": "Express Shipping: Base(30,000) × 1.2 + PerKg(8,000) × Weight(2.00kg) + PeakHourSurge(15,000) = 88,000 VND"
}
```

### Bước 5: Xem lịch sử đơn hàng
```http
GET /api/orders

GET /api/orders/1
```

## 🎨 Design Patterns - Nguyên tắc thiết kế

### ✅ Strategy Pattern

**Ưu điểm:**
1. **Open/Closed Principle** - Thêm strategy mới không cần sửa code cũ
2. **Single Responsibility** - Mỗi strategy lo 1 cách tính phí
3. **Runtime Selection** - User chọn strategy khi checkout
4. **Easy Testing** - Test từng strategy độc lập

**4 Strategies:**
- `StandardShippingStrategy` - Giao hàng tiêu chuẩn (3-5 ngày)
- `ExpressShippingStrategy` - Giao hàng nhanh (1-2 ngày)
- `SameDayShippingStrategy` - Giao trong ngày (trước 2PM)
- `EcoShippingStrategy` - Tiết kiệm (bulk discount ≥10kg)

### ✅ Decorator Pattern (NEW!)

**Ưu điểm:**
1. **Flexible Composition** - Kết hợp nhiều decorators tùy ý
2. **No Subclass Explosion** - Không cần tạo class cho mỗi tổ hợp
3. **Dynamic Addition** - Thêm tính năng runtime
4. **Single Responsibility** - Mỗi decorator lo 1 tính năng

**6 Decorators:**
- `InsuranceDecorator` - Bảo hiểm (~2% giá trị đơn hàng)
- `GiftWrappingDecorator` - Gói quà ($3.50/item + $1.50 thiệp)
- `SignatureRequiredDecorator` - Yêu cầu chữ ký ($2.50-$4.00)
- `PriorityHandlingDecorator` - Xử lý ưu tiên ($5.00 + $0.50/kg)
- `WeekendDeliveryDecorator` - Giao cuối tuần ($6-$10)
- Có thể thêm decorators mới...

**Ví dụ stacking:**
```csharp
IShippingStrategy shipping = new ExpressShippingStrategy();
shipping = new InsuranceDecorator(shipping, orderValue: 120);
shipping = new GiftWrappingDecorator(shipping, itemCount: 2);
shipping = new WeekendDeliveryDecorator(shipping, DayOfWeek.Saturday);

decimal totalFee = shipping.Calculate(context);
// Express ($15) + Insurance ($2.40) + Gift Wrap ($5) + Weekend ($8) = $30.40
```

## 📈 Phát triển dự án

### ✅ Phase 1: Core Features (Hoàn thành)
- Authentication & Authorization (JWT)
- Product CRUD, Cart management
- 4 Shipping Strategies (Strategy Pattern)
- Basic checkout flow
- Unit tests (26 tests)

### ✅ Phase 2: Advanced Features (Hoàn thành)
- 6 Shipping Decorators (Decorator Pattern) - **MỚI!**
- 2 API endpoints cho decorators
- Admin shipping config
- Order history & reports
- Tax & discount calculation

### ✅ Phase 3: Polish & Organization (Hoàn thành)
- Tổ chức lại thành 3 thư mục rõ ràng
- Build executable cho end-users
- Tối giản hóa tài liệu (1 file README.md duy nhất)
- Database scripts & seed data
- Production ready

### 🔮 Future Enhancements
- Payment gateway integration
- Email notifications
- Real-time order tracking
- Mobile app (Flutter/React Native)
- AI-based shipping recommendations

## 🔐 Security

- **JWT Authentication** - Token-based, 24h expiration
- **Role-based Authorization** - User vs Admin
- **Password Hashing** - BCrypt
- **Input Validation** - Data annotations

## 🗂️ Database Schema

### Users
- Id, Email (unique), PasswordHash, Role, IsActive

### Products
- Id, Name, Category, Price, Weight, Stock, IsActive

### ShippingMethods
- Id, Code (unique), DisplayName, ParamsJSON, IsActive

### Orders
- Id, UserId, Subtotal, Discount, Tax, ShippingFee, GrandTotal
- MethodCode, Status, TotalWeight, Distance, Region
- ShippingCalculationDetails (JSON)

### OrderItems
- Id, OrderId, ProductId, Quantity, UnitPrice, LineTotal

### CartItems
- Id, UserId, ProductId, Quantity

## 📞 Default Accounts

### Admin
- Email: `admin@ecommerce.com`
- Password: `admin123`

### User
- Email: `user@ecommerce.com`
- Password: `user123`

## 🎓 Key Learnings

### Design Patterns
1. **Strategy Pattern** - Dễ dàng thêm/thay đổi thuật toán
2. **Decorator Pattern** - Kết hợp tính năng linh hoạt
3. **Open/Closed Principle** - Mở rộng không cần sửa code cũ
4. **Dependency Inversion** - Phụ thuộc vào abstraction

### Best Practices
1. **Clean Architecture** - Tách biệt Domain, Infrastructure, Application
2. **Unit Testing** - Test coverage cho business logic
3. **API Documentation** - Swagger UI tự động
4. **Configuration Management** - JSON params cho flexibility
5. **Project Organization** - 3 thư mục rõ ràng (Source, Executable, Database)

## � Thư mục chi tiết

### 1_Source/
Mã nguồn đầy đủ với Clean Architecture:
- **Core:** Domain entities, interfaces, strategies, decorators
- **Infrastructure:** Database context, data access
- **API:** Controllers, services, DTOs
- **Tests:** 26 unit tests

### 2_Executable/
File chạy trực tiếp (không cần build):
- `MiniECommerce.API.exe` - Windows executable
- `run.ps1` / `run.sh` - Scripts để chạy nhanh
- Tất cả DLLs và dependencies

### 3_Database/
Database và SQL scripts:
- `ecommerce.db` - SQLite database với sample data
- `schema.sql` - Định nghĩa bảng
- `seed_data.sql` - Dữ liệu mẫu (8 products, 2 users, 4 methods)
- `reset_database.sql` - Reset về trạng thái ban đầu

## 📞 Liên hệ & Hỗ trợ

**Tài khoản mặc định:**
- Admin: `admin@ecommerce.com` / `admin123`
- User: `user@ecommerce.com` / `user123`

**Công nghệ:**
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQLite
- JWT Authentication
- Swagger/OpenAPI
- xUnit Testing

## �📝 License

MIT License - Free to use for educational purposes

---

**Developed with ❤️ using ASP.NET Core 8.0**  
**Design Patterns: Strategy + Decorator**  
**Architecture: Clean Architecture (3 Layers)**
