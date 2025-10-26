using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Interfaces;

/// <summary>
/// Strategy Pattern Interface for Shipping Fee Calculation
/// </summary>
public interface IShippingStrategy
{
    /// <summary>
    /// Calculate shipping fee based on order context
    /// </summary>
    decimal Calculate(OrderContext context);
    
    /// <summary>
    /// Strategy name for identification
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Get detailed explanation of the calculation
    /// </summary>
    string GetCalculationDetails(OrderContext context);
}
