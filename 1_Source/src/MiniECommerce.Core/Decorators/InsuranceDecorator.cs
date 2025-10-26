using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Decorators;

/// <summary>
/// Adds shipping insurance to the base shipping cost
/// Decorator Pattern: Adds insurance feature without modifying the original strategy
/// </summary>
public class InsuranceDecorator : ShippingDecorator
{
    private readonly decimal _insuranceRate;
    private readonly decimal _minimumInsurance;

    public InsuranceDecorator(IShippingStrategy strategy, decimal insuranceRate = 0.02m, decimal minimumInsurance = 2.00m)
        : base(strategy)
    {
        _insuranceRate = insuranceRate;
        _minimumInsurance = minimumInsurance;
    }

    public override string Name => $"{_wrappedStrategy.Name} + Insurance";

    public override decimal Calculate(OrderContext context)
    {
        var baseShipping = _wrappedStrategy.Calculate(context);
        var insuranceCost = CalculateInsurance(context);
        return baseShipping + insuranceCost;
    }

    public override string GetCalculationDetails(OrderContext context)
    {
        var baseDetails = _wrappedStrategy.GetCalculationDetails(context);
        var insuranceCost = CalculateInsurance(context);
        var orderValue = context.Weight * 100; // Estimate order value based on weight
        
        return $"{baseDetails}\n" +
               $"Insurance: Order value ~${orderValue:F2} × {_insuranceRate * 100}% = ${insuranceCost:F2}";
    }

    private decimal CalculateInsurance(OrderContext context)
    {
        var orderValue = (decimal)context.Weight * 100m; // Rough estimate: $100 per kg
        var insurance = orderValue * _insuranceRate;
        return Math.Max(insurance, _minimumInsurance);
    }
}
