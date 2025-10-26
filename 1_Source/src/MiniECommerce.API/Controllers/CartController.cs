using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.API.DTOs;
using MiniECommerce.API.Services;
using System.Security.Claims;

namespace MiniECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }
    
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        return int.Parse(userIdClaim ?? "0");
    }
    
    /// <summary>
    /// Get current user's cart
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }
    
    /// <summary>
    /// Add item to cart
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem([FromBody] AddToCartDto dto)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.AddItemAsync(userId, dto);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Update cart item quantity
    /// </summary>
    [HttpPatch("items/{id}")]
    public async Task<ActionResult<CartDto>> UpdateItem(int id, [FromBody] UpdateCartItemDto dto)
    {
        try
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateItemAsync(userId, id, dto);
            return Ok(cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Remove item from cart
    /// </summary>
    [HttpDelete("items/{id}")]
    public async Task<ActionResult> DeleteItem(int id)
    {
        try
        {
            var userId = GetUserId();
            await _cartService.DeleteItemAsync(userId, id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    /// <summary>
    /// Clear cart
    /// </summary>
    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        var userId = GetUserId();
        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
}
