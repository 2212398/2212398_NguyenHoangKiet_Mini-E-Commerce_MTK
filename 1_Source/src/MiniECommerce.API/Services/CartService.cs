using Microsoft.EntityFrameworkCore;
using MiniECommerce.API.DTOs;
using MiniECommerce.Core.Entities;
using MiniECommerce.Infrastructure.Data;

namespace MiniECommerce.API.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;
    
    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
        
        return MapToCartDto(cartItems);
    }
    
    public async Task<CartDto> AddItemAsync(int userId, AddToCartDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId);
        
        if (product == null || !product.IsActive)
        {
            throw new InvalidOperationException("Product not found or inactive");
        }
        
        if (product.Stock < dto.Quantity)
        {
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}");
        }
        
        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            _context.CartItems.Add(newItem);
        }
        
        await _context.SaveChangesAsync();
        
        return await GetCartAsync(userId);
    }
    
    public async Task<CartDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
    {
        var cartItem = await _context.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        
        if (cartItem == null)
        {
            throw new InvalidOperationException("Cart item not found");
        }
        
        if (cartItem.Product!.Stock < dto.Quantity)
        {
            throw new InvalidOperationException($"Insufficient stock. Available: {cartItem.Product.Stock}");
        }
        
        cartItem.Quantity = dto.Quantity;
        cartItem.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return await GetCartAsync(userId);
    }
    
    public async Task DeleteItemAsync(int userId, int cartItemId)
    {
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
        
        if (cartItem == null)
        {
            throw new InvalidOperationException("Cart item not found");
        }
        
        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();
    }
    
    public async Task ClearCartAsync(int userId)
    {
        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();
        
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }
    
    private CartDto MapToCartDto(List<CartItem> items)
    {
        var cartItemDtos = items.Select(item => new CartItemDto
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.Product?.Name ?? "",
            UnitPrice = item.Product?.Price ?? 0,
            Weight = item.Product?.Weight ?? 0,
            Quantity = item.Quantity,
            LineTotal = (item.Product?.Price ?? 0) * item.Quantity
        }).ToList();
        
        return new CartDto
        {
            Items = cartItemDtos,
            Subtotal = cartItemDtos.Sum(i => i.LineTotal),
            TotalWeight = cartItemDtos.Sum(i => i.Weight * i.Quantity)
        };
    }
}
