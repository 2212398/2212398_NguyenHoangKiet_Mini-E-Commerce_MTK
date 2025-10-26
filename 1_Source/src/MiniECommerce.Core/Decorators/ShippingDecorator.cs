using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Decorators;

/// <summary>
/// Base Decorator for IShippingStrategy
/// Implements Decorator Pattern to add additional features to shipping strategies
/// </summary>
public abstract class ShippingDecorator : IShippingStrategy
{
    protected readonly IShippingStrategy _wrappedStrategy;

    protected ShippingDecorator(IShippingStrategy strategy)
    {
        _wrappedStrategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
    }

    public virtual string Name => _wrappedStrategy.Name;

    public virtual decimal Calculate(OrderContext context)
    {
        return _wrappedStrategy.Calculate(context);
    }

    public virtual string GetCalculationDetails(OrderContext context)
    {
        return _wrappedStrategy.GetCalculationDetails(context);
    }
}
