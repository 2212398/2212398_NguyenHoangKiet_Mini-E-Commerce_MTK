using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;
using System.Text.Json;

namespace MiniECommerce.Core.Strategies;

/// <summary>
/// Eco shipping: Cheaper but longer delivery, encourages bulk orders
/// </summary>
public class EcoShippingStrategy : IShippingStrategy
{
    public string Name => "Eco Shipping";
    
    private readonly decimal _baseFee;
    private readonly decimal _perKgFee;
    private readonly double _bulkWeightThreshold;
    private readonly decimal _bulkDiscount;
    
    public EcoShippingStrategy(string? paramsJson = null)
    {
        // Default values
        _baseFee = 15000;
        _perKgFee = 3000;
        _bulkWeightThreshold = 10.0; // kg
        _bulkDiscount = 0.15m; // 15% discount
        
        if (!string.IsNullOrEmpty(paramsJson))
        {
            try
            {
                var options = JsonSerializer.Deserialize<EcoShippingParams>(paramsJson);
                if (options != null)
                {
                    _baseFee = options.BaseFee;
                    _perKgFee = options.PerKgFee;
                    _bulkWeightThreshold = options.BulkWeightThreshold;
                    _bulkDiscount = options.BulkDiscount;
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
        decimal fee = _baseFee + (_perKgFee * (decimal)context.Weight);
        
        // Apply bulk discount if weight exceeds threshold
        if (context.Weight >= _bulkWeightThreshold)
        {
            fee *= (1 - _bulkDiscount);
        }
        
        return Math.Round(fee, 0);
    }
    
    public string GetCalculationDetails(OrderContext context)
    {
        bool isBulk = context.Weight >= _bulkWeightThreshold;
        string discountText = isBulk ? $" × (1 - BulkDiscount({_bulkDiscount:P0}))" : "";
        string bulkNote = isBulk ? $" [Bulk order ≥ {_bulkWeightThreshold}kg]" : "";
        
        return $"Eco Shipping: Base({_baseFee:N0}) + PerKg({_perKgFee:N0}) × Weight({context.Weight:N2}kg){discountText} = {Calculate(context):N0} VND{bulkNote}";
    }
    
    private class EcoShippingParams
    {
        public decimal BaseFee { get; set; }
        public decimal PerKgFee { get; set; }
        public double BulkWeightThreshold { get; set; }
        public decimal BulkDiscount { get; set; }
    }
}
