using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Decorators;

/// <summary>
/// Adds weekend delivery option
/// Decorator Pattern: Adds weekend delivery without modifying the original strategy
/// </summary>
public class WeekendDeliveryDecorator : ShippingDecorator
{
    private readonly decimal _weekendSurcharge;
    private readonly decimal _saturdayFee;
    private readonly decimal _sundayFee;
    private readonly DayOfWeek _preferredDay;

    public WeekendDeliveryDecorator(
        IShippingStrategy strategy,
        DayOfWeek preferredDay = DayOfWeek.Saturday,
        decimal weekendSurcharge = 8.00m,
        decimal saturdayFee = 6.00m,
        decimal sundayFee = 10.00m)
        : base(strategy)
    {
        _preferredDay = preferredDay;
        _weekendSurcharge = weekendSurcharge;
        _saturdayFee = saturdayFee;
        _sundayFee = sundayFee;
    }

    public override string Name => $"{_wrappedStrategy.Name} + Weekend Delivery ({_preferredDay})";

    public override decimal Calculate(OrderContext context)
    {
        var baseShipping = _wrappedStrategy.Calculate(context);
        var weekendCost = CalculateWeekendCost();
        return baseShipping + weekendCost;
    }

    public override string GetCalculationDetails(OrderContext context)
    {
        var baseDetails = _wrappedStrategy.GetCalculationDetails(context);
        var weekendCost = CalculateWeekendCost();
        var dayName = _preferredDay == DayOfWeek.Saturday ? "Saturday" : "Sunday";
        
        return $"{baseDetails}\n" +
               $"Weekend Delivery ({dayName}): ${weekendCost:F2}";
    }

    private decimal CalculateWeekendCost()
    {
        return _preferredDay == DayOfWeek.Saturday ? _saturdayFee : _sundayFee;
    }
}
