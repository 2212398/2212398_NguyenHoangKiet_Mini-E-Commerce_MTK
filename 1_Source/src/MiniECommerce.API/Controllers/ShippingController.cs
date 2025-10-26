using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.API.DTOs;
using MiniECommerce.API.Services;
using MiniECommerce.Core.Models;
using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Decorators;
using MiniECommerce.Core.Strategies;

namespace MiniECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;
    private readonly ICartService _cartService;
    
    public ShippingController(IShippingService shippingService, ICartService cartService)
    {
        _shippingService = shippingService;
        _cartService = cartService;
    }
    
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
    
    /// <summary>
    /// Get available shipping options with estimated fees
    /// Uses Strategy Pattern to calculate fees
    /// </summary>
    [HttpPost("options")]
    [Authorize]
    public async Task<ActionResult<List<ShippingOptionDto>>> GetShippingOptions([FromBody] GetShippingOptionsDto dto)
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        
        var context = new OrderContext
        {
            Weight = dto.Weight > 0 ? dto.Weight : cart.TotalWeight,
            Distance = dto.Distance,
            Region = dto.Region,
            Subtotal = cart.Subtotal,
            OrderTime = DateTime.Now
        };
        
        var options = await _shippingService.GetShippingOptionsAsync(context);
        return Ok(options);
    }
    
    /// <summary>
    /// Get all shipping methods (Admin only)
    /// </summary>
    [HttpGet("methods")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ShippingMethodDto>>> GetAllMethods()
    {
        var methods = await _shippingService.GetAllMethodsAsync();
        return Ok(methods);
    }
    
    /// <summary>
    /// Update shipping method configuration (Admin only)
    /// </summary>
    [HttpPut("methods/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ShippingMethodDto>> UpdateMethod(int id, [FromBody] UpdateShippingMethodDto dto)
    {
        try
        {
            var method = await _shippingService.UpdateMethodAsync(id, dto);
            return Ok(method);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get shipping options with add-ons using Decorator Pattern
    /// Demonstrates how decorators add features without modifying base strategies
    /// </summary>
    [HttpPost("options-with-addons")]
    [Authorize]
    public async Task<ActionResult> GetShippingOptionsWithAddons([FromBody] GetShippingWithAddonsDto dto)
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        
        var context = new OrderContext
        {
            Weight = dto.Weight > 0 ? dto.Weight : cart.TotalWeight,
            Distance = dto.Distance,
            Region = dto.Region,
            Subtotal = cart.Subtotal,
            OrderTime = DateTime.Now
        };
        
        // Get base strategy
        IShippingStrategy baseStrategy = dto.MethodCode.ToUpper() switch
        {
            "STANDARD" => new StandardShippingStrategy("{\"BaseRate\":5.00,\"WeightRate\":2.00,\"WeightThreshold\":5.0,\"DistanceRate\":0.50}"),
            "EXPRESS" => new ExpressShippingStrategy("{\"BaseRate\":15.00,\"WeightRate\":3.00,\"DistanceMultiplier\":1.5}"),
            "SAME_DAY" => new SameDayShippingStrategy("{\"BaseRate\":25.00,\"CutoffTime\":14}"),
            "ECO" => new EcoShippingStrategy("{\"BaseRate\":3.00,\"WeightRate\":1.50,\"BulkDiscount\":0.10,\"BulkThreshold\":10.0}"),
            _ => throw new InvalidOperationException($"Unknown method code: {dto.MethodCode}")
        };
        
        // Apply decorators based on selected add-ons
        var decoratedStrategy = baseStrategy;
        var appliedAddons = new List<string>();
        
        if (dto.AddInsurance)
        {
            decoratedStrategy = new InsuranceDecorator(decoratedStrategy);
            appliedAddons.Add("Insurance");
        }
        
        if (dto.AddGiftWrapping)
        {
            decoratedStrategy = new GiftWrappingDecorator(decoratedStrategy);
            appliedAddons.Add("Gift Wrapping");
        }
        
        if (dto.RequireSignature)
        {
            decoratedStrategy = new SignatureRequiredDecorator(decoratedStrategy, dto.RequireAdultSignature);
            appliedAddons.Add(dto.RequireAdultSignature ? "Adult Signature" : "Signature");
        }
        
        if (dto.AddPriorityHandling)
        {
            decoratedStrategy = new PriorityHandlingDecorator(decoratedStrategy);
            appliedAddons.Add("Priority Handling");
        }
        
        if (dto.WeekendDelivery.HasValue)
        {
            decoratedStrategy = new WeekendDeliveryDecorator(decoratedStrategy, dto.WeekendDelivery.Value);
            appliedAddons.Add($"Weekend Delivery ({dto.WeekendDelivery.Value})");
        }
        
        // Calculate final price
        var totalFee = decoratedStrategy.Calculate(context);
        var details = decoratedStrategy.GetCalculationDetails(context);
        
        return Ok(new
        {
            methodCode = dto.MethodCode,
            methodName = decoratedStrategy.Name,
            baseFee = baseStrategy.Calculate(context),
            totalFee = totalFee,
            addonsApplied = appliedAddons,
            addonsCount = appliedAddons.Count,
            calculationDetails = details,
            savings = totalFee > 0 ? $"You're adding ${totalFee - baseStrategy.Calculate(context):F2} in premium services" : null
        });
    }
    
    /// <summary>
    /// Demo endpoint showing all possible decorator combinations
    /// Educational purpose: Shows Decorator Pattern flexibility
    /// </summary>
    [HttpPost("decorator-demo")]
    [Authorize]
    public ActionResult GetDecoratorDemo([FromBody] GetShippingOptionsDto dto)
    {
        var context = new OrderContext
        {
            Weight = dto.Weight,
            Distance = dto.Distance,
            Region = dto.Region,
            Subtotal = 100m,
            OrderTime = DateTime.Now
        };
        
        var baseStrategy = new StandardShippingStrategy("{\"BaseRate\":5.00,\"WeightRate\":2.00,\"WeightThreshold\":5.0,\"DistanceRate\":0.50}");
        var baseFee = baseStrategy.Calculate(context);
        
        var examples = new List<object>
        {
            new
            {
                description = "Base Strategy (No decorators)",
                name = baseStrategy.Name,
                fee = baseFee,
                details = baseStrategy.GetCalculationDetails(context)
            },
            new
            {
                description = "Base + Insurance",
                name = new InsuranceDecorator(baseStrategy).Name,
                fee = new InsuranceDecorator(baseStrategy).Calculate(context),
                details = new InsuranceDecorator(baseStrategy).GetCalculationDetails(context)
            },
            new
            {
                description = "Base + Gift Wrapping",
                name = new GiftWrappingDecorator(baseStrategy).Name,
                fee = new GiftWrappingDecorator(baseStrategy).Calculate(context),
                details = new GiftWrappingDecorator(baseStrategy).GetCalculationDetails(context)
            },
            new
            {
                description = "Base + Insurance + Gift Wrapping",
                name = new GiftWrappingDecorator(new InsuranceDecorator(baseStrategy)).Name,
                fee = new GiftWrappingDecorator(new InsuranceDecorator(baseStrategy)).Calculate(context),
                details = new GiftWrappingDecorator(new InsuranceDecorator(baseStrategy)).GetCalculationDetails(context)
            },
            new
            {
                description = "Base + All Add-ons (Full Decorator Stack)",
                name = new WeekendDeliveryDecorator(
                    new PriorityHandlingDecorator(
                        new SignatureRequiredDecorator(
                            new GiftWrappingDecorator(
                                new InsuranceDecorator(baseStrategy)
                            )
                        )
                    )
                ).Name,
                fee = new WeekendDeliveryDecorator(
                    new PriorityHandlingDecorator(
                        new SignatureRequiredDecorator(
                            new GiftWrappingDecorator(
                                new InsuranceDecorator(baseStrategy)
                            )
                        )
                    )
                ).Calculate(context),
                details = new WeekendDeliveryDecorator(
                    new PriorityHandlingDecorator(
                        new SignatureRequiredDecorator(
                            new GiftWrappingDecorator(
                                new InsuranceDecorator(baseStrategy)
                            )
                        )
                    )
                ).GetCalculationDetails(context)
            }
        };
        
        return Ok(new
        {
            message = "Decorator Pattern Demo - Shows how decorators stack to add features",
            baseFee = baseFee,
            examples = examples,
            learningPoints = new[]
            {
                "Decorators wrap the base strategy without modifying it",
                "Multiple decorators can be stacked (composed)",
                "Each decorator adds its own calculation and details",
                "Order of decoration can matter for calculation",
                "Base strategy remains unchanged and reusable"
            }
        });
    }
}
