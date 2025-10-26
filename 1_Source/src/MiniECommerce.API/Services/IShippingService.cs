using MiniECommerce.API.DTOs;
using MiniECommerce.Core.Models;

namespace MiniECommerce.API.Services;

public interface IShippingService
{
    Task<List<ShippingOptionDto>> GetShippingOptionsAsync(OrderContext context);
    Task<decimal> CalculateShippingFeeAsync(string methodCode, OrderContext context);
    Task<string> GetCalculationDetailsAsync(string methodCode, OrderContext context);
    Task<List<ShippingMethodDto>> GetAllMethodsAsync();
    Task<ShippingMethodDto> UpdateMethodAsync(int id, UpdateShippingMethodDto dto);
}
