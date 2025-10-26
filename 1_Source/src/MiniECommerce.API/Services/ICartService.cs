using MiniECommerce.API.DTOs;

namespace MiniECommerce.API.Services;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<CartDto> AddItemAsync(int userId, AddToCartDto dto);
    Task<CartDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
    Task DeleteItemAsync(int userId, int cartItemId);
    Task ClearCartAsync(int userId);
}
