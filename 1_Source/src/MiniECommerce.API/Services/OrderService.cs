using Microsoft.EntityFrameworkCore;
using MiniECommerce.API.DTOs;
using MiniECommerce.Core.Entities;
using MiniECommerce.Core.Models;
using MiniECommerce.Infrastructure.Data;

namespace MiniECommerce.API.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IShippingService _shippingService;
    private readonly ICartService _cartService;
    private readonly IConfiguration _configuration;
    
    public OrderService(
        ApplicationDbContext context,
        IShippingService shippingService,
        ICartService cartService,
        IConfiguration configuration)
    {
        _context = context;
        _shippingService = shippingService;
        _cartService = cartService;
        _configuration = configuration;
    }
    
    public async Task<CheckoutResponseDto> CheckoutAsync(int userId, CheckoutDto dto)
    {
        // Get cart
        var cart = await _cartService.GetCartAsync(userId);
        
        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty");
        }
        
        // Calculate subtotal
        decimal subtotal = cart.Subtotal;
        
        // Calculate discount (simple fixed logic)
        decimal discount = 0;
        if (!string.IsNullOrEmpty(dto.DiscountCode))
        {
            discount = CalculateDiscount(subtotal, dto.DiscountCode);
        }
        
        // Calculate tax (fixed rate from config)
        decimal taxRate = _configuration.GetValue<decimal>("AppSettings:TaxRate");
        decimal tax = subtotal * taxRate;
        
        // Create order context for shipping calculation
        var orderContext = new OrderContext
        {
            Weight = cart.TotalWeight,
            Distance = dto.Distance,
            Region = dto.Region,
            Subtotal = subtotal,
            OrderTime = DateTime.Now
        };
        
        // Calculate shipping fee using Strategy Pattern
        decimal shippingFee = await _shippingService.CalculateShippingFeeAsync(dto.ShippingMethodCode, orderContext);
        string calculationDetails = await _shippingService.GetCalculationDetailsAsync(dto.ShippingMethodCode, orderContext);
        
        // Calculate grand total
        decimal grandTotal = subtotal - discount + tax + shippingFee;
        
        // Create order
        var order = new Order
        {
            UserId = userId,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            ShippingFee = shippingFee,
            GrandTotal = grandTotal,
            MethodCode = dto.ShippingMethodCode,
            Status = "Pending",
            TotalWeight = cart.TotalWeight,
            Distance = dto.Distance,
            Region = dto.Region,
            ShippingCalculationDetails = calculationDetails
        };
        
        // Add order items
        foreach (var cartItem in cart.Items)
        {
            var product = await _context.Products.FindAsync(cartItem.ProductId);
            
            if (product == null || product.Stock < cartItem.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product: {cartItem.ProductName}");
            }
            
            order.Items.Add(new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.UnitPrice,
                LineTotal = cartItem.LineTotal
            });
            
            // Update stock
            product.Stock -= cartItem.Quantity;
        }
        
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        
        // Clear cart
        await _cartService.ClearCartAsync(userId);
        
        return new CheckoutResponseDto
        {
            OrderId = order.Id,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            TaxRate = taxRate,
            ShippingFee = shippingFee,
            ShippingMethodCode = dto.ShippingMethodCode,
            ShippingCalculationDetails = calculationDetails,
            GrandTotal = grandTotal,
            Status = order.Status
        };
    }
    
    public async Task<List<OrderSummaryDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _context.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                GrandTotal = o.GrandTotal,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ShippingMethodCode = o.MethodCode,
                ItemCount = o.Items.Count
            })
            .ToListAsync();
        
        return orders;
    }
    
    public async Task<OrderDetailDto> GetOrderDetailAsync(int userId, int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        
        if (order == null)
        {
            throw new InvalidOperationException("Order not found");
        }
        
        return new OrderDetailDto
        {
            Id = order.Id,
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            Tax = order.Tax,
            ShippingFee = order.ShippingFee,
            GrandTotal = order.GrandTotal,
            ShippingMethodCode = order.MethodCode,
            ShippingCalculationDetails = order.ShippingCalculationDetails ?? "",
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductName = i.Product?.Name ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.LineTotal
            }).ToList()
        };
    }
    
    private decimal CalculateDiscount(decimal subtotal, string discountCode)
    {
        // Simple fixed discount logic (not stacking)
        var maxDiscount = _configuration.GetValue<decimal>("AppSettings:MaxDiscountAmount");
        
        return discountCode.ToUpper() switch
        {
            "SAVE10" => Math.Min(subtotal * 0.10m, maxDiscount),
            "SAVE50K" => Math.Min(50000m, maxDiscount),
            "VIP20" => Math.Min(subtotal * 0.20m, maxDiscount),
            _ => 0
        };
    }
}
