namespace MiniECommerce.Core.Models;

/// <summary>
/// Context information for shipping fee calculation
/// </summary>
public class OrderContext
{
    public double Weight { get; set; } // Total weight in kg
    public double Distance { get; set; } // Distance in km
    public string Region { get; set; } = string.Empty; // North, South, Central
    public decimal Subtotal { get; set; }
    public DateTime? RequestedDeliveryTime { get; set; }
    public DateTime OrderTime { get; set; } = DateTime.Now;
}
