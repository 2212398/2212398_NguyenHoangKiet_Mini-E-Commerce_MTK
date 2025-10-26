namespace MiniECommerce.API.DTOs;

// Auth DTOs
public class RegisterDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

// Product DTOs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Weight { get; set; }
    public int Stock { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Weight { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public double Weight { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

// Cart DTOs
public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public double TotalWeight { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public double Weight { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemDto
{
    public int Quantity { get; set; }
}

// Shipping DTOs
public class ShippingOptionDto
{
    public string MethodCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal EstimatedFee { get; set; }
    public string CalculationDetails { get; set; } = string.Empty;
}

public class ShippingMethodDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ParamsJSON { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class UpdateShippingMethodDto
{
    public string DisplayName { get; set; } = string.Empty;
    public string ParamsJSON { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class GetShippingOptionsDto
{
    public double Weight { get; set; }
    public double Distance { get; set; }
    public string Region { get; set; } = string.Empty;
}

// Order DTOs
public class CheckoutDto
{
    public string ShippingMethodCode { get; set; } = string.Empty;
    public string Region { get; set; } = "North";
    public double Distance { get; set; } = 10.0;
    public string? DiscountCode { get; set; }
}

public class CheckoutResponseDto
{
    public int OrderId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal ShippingFee { get; set; }
    public string ShippingMethodCode { get; set; } = string.Empty;
    public string ShippingCalculationDetails { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class OrderSummaryDto
{
    public int Id { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string ShippingMethodCode { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}

public class OrderDetailDto
{
    public int Id { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal GrandTotal { get; set; }
    public string ShippingMethodCode { get; set; } = string.Empty;
    public string ShippingCalculationDetails { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

// Report DTOs
public class DailyReportDto
{
    public DateTime Date { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalShippingFees { get; set; }
    public decimal AverageOrderValue { get; set; }
    public List<ShippingMethodStats> OrdersByMethod { get; set; } = new();
}

public class ShippingMethodStats
{
    public string MethodCode { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalFees { get; set; }
}

// Decorator Pattern DTOs
public class GetShippingWithAddonsDto
{
    public string MethodCode { get; set; } = "STANDARD";
    public double Weight { get; set; }
    public double Distance { get; set; }
    public string Region { get; set; } = "North";
    
    // Decorator add-ons
    public bool AddInsurance { get; set; }
    public bool AddGiftWrapping { get; set; }
    public bool RequireSignature { get; set; }
    public bool RequireAdultSignature { get; set; }
    public bool AddPriorityHandling { get; set; }
    public DayOfWeek? WeekendDelivery { get; set; } // Saturday or Sunday
}
