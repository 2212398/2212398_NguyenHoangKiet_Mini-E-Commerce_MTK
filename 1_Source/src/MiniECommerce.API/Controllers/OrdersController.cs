using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.API.DTOs;
using MiniECommerce.API.Services;

namespace MiniECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
    
    /// <summary>
    /// Checkout - Create order with shipping fee calculated by Strategy Pattern
    /// </summary>
    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutResponseDto>> Checkout([FromBody] CheckoutDto dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.CheckoutAsync(userId, dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Get user's order history
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<OrderSummaryDto>>> GetOrders()
    {
        var userId = GetUserId();
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }
    
    /// <summary>
    /// Get order detail including shipping calculation details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrder(int id)
    {
        try
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderDetailAsync(userId, id);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
