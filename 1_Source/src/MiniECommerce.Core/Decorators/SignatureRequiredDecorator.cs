using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;

namespace MiniECommerce.Core.Decorators;

/// <summary>
/// Adds signature requirement for delivery
/// Decorator Pattern: Adds signature verification without modifying the original strategy
/// </summary>
public class SignatureRequiredDecorator : ShippingDecorator
{
    private readonly decimal _signatureFee;
    private readonly decimal _adultSignatureFee;
    private readonly bool _requireAdultSignature;

    public SignatureRequiredDecorator(
        IShippingStrategy strategy, 
        bool requireAdultSignature = false,
        decimal signatureFee = 2.50m,
        decimal adultSignatureFee = 4.00m)
        : base(strategy)
    {
        _requireAdultSignature = requireAdultSignature;
        _signatureFee = signatureFee;
        _adultSignatureFee = adultSignatureFee;
    }

    public override string Name => _requireAdultSignature 
        ? $"{_wrappedStrategy.Name} + Adult Signature Required"
        : $"{_wrappedStrategy.Name} + Signature Required";

    public override decimal Calculate(OrderContext context)
    {
        var baseShipping = _wrappedStrategy.Calculate(context);
        var signatureCost = _requireAdultSignature ? _adultSignatureFee : _signatureFee;
        return baseShipping + signatureCost;
    }

    public override string GetCalculationDetails(OrderContext context)
    {
        var baseDetails = _wrappedStrategy.GetCalculationDetails(context);
        var signatureType = _requireAdultSignature ? "Adult Signature" : "Signature";
        var signatureCost = _requireAdultSignature ? _adultSignatureFee : _signatureFee;
        
        return $"{baseDetails}\n" +
               $"{signatureType} Required: ${signatureCost:F2}";
    }
}
