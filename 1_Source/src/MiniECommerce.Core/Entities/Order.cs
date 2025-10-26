namespace MiniECommerce.Core.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal GrandTotal { get; set; }
    public string MethodCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public User? User { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    
    // Additional info for shipping calculation
    public double TotalWeight { get; set; }
    public double Distance { get; set; }
    public string Region { get; set; } = string.Empty;
    public string? ShippingCalculationDetails { get; set; } // JSON explaining fee breakdown
}
