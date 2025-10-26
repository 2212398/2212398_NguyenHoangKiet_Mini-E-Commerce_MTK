namespace MiniECommerce.Core.Entities;

public class ShippingMethod
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // STANDARD, EXPRESS, SAME_DAY, ECO
    public string DisplayName { get; set; } = string.Empty;
    public string ParamsJSON { get; set; } = "{}"; // JSON parameters for strategy
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
