using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Decorators;

/// <summary>
/// Adds gift wrapping service to the shipping
/// Decorator Pattern: Adds gift wrapping feature without modifying the original strategy
/// </summary>
public class GiftWrappingDecorator : ShippingDecorator
{
    private readonly decimal _wrappingFeePerItem;
    private readonly decimal _giftCardFee;

    public GiftWrappingDecorator(
        IShippingStrategy strategy, 
        decimal wrappingFeePerItem = 3.50m, 
        decimal giftCardFee = 1.50m)
        : base(strategy)
    {
        _wrappingFeePerItem = wrappingFeePerItem;
        _giftCardFee = giftCardFee;
    }

    public override string Name => $"{_wrappedStrategy.Name} + Gift Wrapping";

    public override decimal Calculate(OrderContext context)
    {
        var baseShipping = _wrappedStrategy.Calculate(context);
        var wrappingCost = CalculateWrapping(context);
        return baseShipping + wrappingCost;
    }

    public override string GetCalculationDetails(OrderContext context)
    {
        var baseDetails = _wrappedStrategy.GetCalculationDetails(context);
        var itemCount = EstimateItemCount(context);
        var wrappingCost = CalculateWrapping(context);
        
        return $"{baseDetails}\n" +
               $"Gift Wrapping: {itemCount} items × ${_wrappingFeePerItem:F2} + Gift card ${_giftCardFee:F2} = ${wrappingCost:F2}";
    }

    private decimal CalculateWrapping(OrderContext context)
    {
        var itemCount = EstimateItemCount(context);
        return (itemCount * _wrappingFeePerItem) + _giftCardFee;
    }

    private int EstimateItemCount(OrderContext context)
    {
        // Rough estimate: 1 item per 0.5kg
        return Math.Max(1, (int)Math.Ceiling(context.Weight / 0.5));
    }
}
