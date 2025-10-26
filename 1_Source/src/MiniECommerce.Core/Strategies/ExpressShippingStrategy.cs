using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;
using System.Text.Json;

namespace MiniECommerce.Core.Strategies;

/// <summary>
/// Express shipping: base*1.2 + perKg*weight + surgeByTime
/// </summary>
public class ExpressShippingStrategy : IShippingStrategy
{
    public string Name => "Express Shipping";
    
    private readonly decimal _baseFee;
    private readonly decimal _baseMultiplier;
    private readonly decimal _perKgFee;
    private readonly decimal _peakHourSurge;
    
    public ExpressShippingStrategy(string? paramsJson = null)
    {
        // Default values
        _baseFee = 30000;
        _baseMultiplier = 1.2m;
        _perKgFee = 8000;
        _peakHourSurge = 15000;
        
        if (!string.IsNullOrEmpty(paramsJson))
        {
            try
            {
                var options = JsonSerializer.Deserialize<ExpressShippingParams>(paramsJson);
                if (options != null)
                {
                    _baseFee = options.BaseFee;
                    _baseMultiplier = options.BaseMultiplier;
                    _perKgFee = options.PerKgFee;
                    _peakHourSurge = options.PeakHourSurge;
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
        decimal fee = (_baseFee * _baseMultiplier) + (_perKgFee * (decimal)context.Weight);
        
        // Peak hours surge (7-9 AM and 5-7 PM)
        int hour = context.OrderTime.Hour;
        if ((hour >= 7 && hour < 9) || (hour >= 17 && hour < 19))
        {
            fee += _peakHourSurge;
        }
        
        return Math.Round(fee, 0);
    }
    
    public string GetCalculationDetails(OrderContext context)
    {
        int hour = context.OrderTime.Hour;
        bool isPeakHour = (hour >= 7 && hour < 9) || (hour >= 17 && hour < 19);
        string surgeText = isPeakHour ? $" + PeakHourSurge({_peakHourSurge:N0})" : "";
        
        return $"Express Shipping: Base({_baseFee:N0}) × {_baseMultiplier} + PerKg({_perKgFee:N0}) × Weight({context.Weight:N2}kg){surgeText} = {Calculate(context):N0} VND";
    }
    
    private class ExpressShippingParams
    {
        public decimal BaseFee { get; set; }
        public decimal BaseMultiplier { get; set; }
        public decimal PerKgFee { get; set; }
        public decimal PeakHourSurge { get; set; }
    }
}
