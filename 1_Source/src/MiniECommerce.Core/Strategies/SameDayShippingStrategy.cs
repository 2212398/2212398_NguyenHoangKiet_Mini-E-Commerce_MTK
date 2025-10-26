using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;
using System.Text.Json;

namespace MiniECommerce.Core.Strategies;

/// <summary>
/// Same-day shipping: baseHigh + perKm*distance (with cutoff time constraint)
/// </summary>
public class SameDayShippingStrategy : IShippingStrategy
{
    public string Name => "Same-Day Shipping";
    
    private readonly decimal _baseFee;
    private readonly decimal _perKmFee;
    private readonly int _cutoffHour;
    
    public SameDayShippingStrategy(string? paramsJson = null)
    {
        // Default values
        _baseFee = 50000;
        _perKmFee = 3000;
        _cutoffHour = 14; // 2 PM cutoff
        
        if (!string.IsNullOrEmpty(paramsJson))
        {
            try
            {
                var options = JsonSerializer.Deserialize<SameDayShippingParams>(paramsJson);
                if (options != null)
                {
                    _baseFee = options.BaseFee;
                    _perKmFee = options.PerKmFee;
                    _cutoffHour = options.CutoffHour;
                }
            }
            catch
            {
                // Use default values
            }
        }
    }
    
    public decimal Calculate(OrderContext context)
    {
        // Check cutoff time
        if (context.OrderTime.Hour >= _cutoffHour)
        {
            throw new InvalidOperationException($"Same-day delivery not available after {_cutoffHour}:00. Please choose another shipping method.");
        }
        
        decimal fee = _baseFee + (_perKmFee * (decimal)context.Distance);
        
        return Math.Round(fee, 0);
    }
    
    public string GetCalculationDetails(OrderContext context)
    {
        if (context.OrderTime.Hour >= _cutoffHour)
        {
            return $"Same-Day Shipping: Not available (cutoff time: {_cutoffHour}:00)";
        }
        
        return $"Same-Day Shipping: Base({_baseFee:N0}) + PerKm({_perKmFee:N0}) × Distance({context.Distance:N2}km) = {Calculate(context):N0} VND";
    }
    
    private class SameDayShippingParams
    {
        public decimal BaseFee { get; set; }
        public decimal PerKmFee { get; set; }
        public int CutoffHour { get; set; }
    }
}
