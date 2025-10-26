using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Decorators;

/// <summary>
/// Adds priority handling service for fragile or high-value items
/// Decorator Pattern: Adds priority handling without modifying the original strategy
/// </summary>
public class PriorityHandlingDecorator : ShippingDecorator
{
    private readonly decimal _priorityFee;
    private readonly decimal _weightMultiplier;

    public PriorityHandlingDecorator(
        IShippingStrategy strategy, 
        decimal priorityFee = 5.00m,
        decimal weightMultiplier = 0.50m)
        : base(strategy)
    {
        _priorityFee = priorityFee;
        _weightMultiplier = weightMultiplier;
    }

    public override string Name => $"{_wrappedStrategy.Name} + Priority Handling";

    public override decimal Calculate(OrderContext context)
    {
        var baseShipping = _wrappedStrategy.Calculate(context);
        var priorityCost = CalculatePriorityCost(context);
        return baseShipping + priorityCost;
    }

    public override string GetCalculationDetails(OrderContext context)
    {
        var baseDetails = _wrappedStrategy.GetCalculationDetails(context);
        var weightCost = (decimal)context.Weight * _weightMultiplier;
        var totalPriorityCost = _priorityFee + weightCost;
        
        return $"{baseDetails}\n" +
               $"Priority Handling: Base ${_priorityFee:F2} + Weight {context.Weight}kg × ${_weightMultiplier:F2} = ${totalPriorityCost:F2}";
    }

    private decimal CalculatePriorityCost(OrderContext context)
    {
        return _priorityFee + ((decimal)context.Weight * _weightMultiplier);
    }
}
