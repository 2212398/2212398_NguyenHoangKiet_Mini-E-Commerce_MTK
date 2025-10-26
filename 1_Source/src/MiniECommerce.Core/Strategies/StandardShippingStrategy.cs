using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;
using System.Text.Json;

namespace MiniECommerce.Core.Strategies;

/// <summary>
/// Standard shipping: base + perKg*weight + regionFactor
/// </summary>
public class StandardShippingStrategy : IShippingStrategy
{
    public string Name => "Standard Shipping";
    
    private readonly decimal _baseFee;
    private readonly decimal _perKgFee;
    private readonly Dictionary<string, decimal> _regionFactors;
    
    public StandardShippingStrategy(string? paramsJson = null)
    {
        // Default values
        _baseFee = 20000;
        _perKgFee = 5000;
        _regionFactors = new Dictionary<string, decimal>
        {
            { "North", 1.0m },
            { "Central", 1.2m },
            { "South", 1.5m }
        };
        
        if (!string.IsNullOrEmpty(paramsJson))
        {
            try
            {
                var options = JsonSerializer.Deserialize<StandardShippingParams>(paramsJson);
                if (options != null)
                {
                    _baseFee = options.BaseFee;
                    _perKgFee = options.PerKgFee;
                    if (options.RegionFactors != null && options.RegionFactors.Count > 0)
                    {
                        _regionFactors = options.RegionFactors;
                    }
                }
            }
            catch
            {
                // Use default values if parsing fails
            }
        }
    }
    
    public decimal Calculate(OrderContext context)
    {
        decimal regionFactor = _regionFactors.ContainsKey(context.Region) 
            ? _regionFactors[context.Region] 
            : 1.0m;
            
        decimal fee = _baseFee + (_perKgFee * (decimal)context.Weight);
        fee *= regionFactor;
        
        return Math.Round(fee, 0);
    }
    
    public string GetCalculationDetails(OrderContext context)
    {
        decimal regionFactor = _regionFactors.ContainsKey(context.Region) 
            ? _regionFactors[context.Region] 
            : 1.0m;
            
        return $"Standard Shipping: Base({_baseFee:N0}) + PerKg({_perKgFee:N0}) × Weight({context.Weight:N2}kg) × RegionFactor[{context.Region}]({regionFactor:N2}) = {Calculate(context):N0} VND";
    }
    
    private class StandardShippingParams
    {
        public decimal BaseFee { get; set; }
        public decimal PerKgFee { get; set; }
        public Dictionary<string, decimal> RegionFactors { get; set; } = new();
    }
}
