using MiniECommerce.API.DTOs;

namespace MiniECommerce.API.Services;

public interface IOrderService
{
    Task<CheckoutResponseDto> CheckoutAsync(int userId, CheckoutDto dto);
    Task<List<OrderSummaryDto>> GetUserOrdersAsync(int userId);
    Task<OrderDetailDto> GetOrderDetailAsync(int userId, int orderId);
}
