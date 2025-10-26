using MiniECommerce.API.DTOs;

namespace MiniECommerce.API.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetProductsAsync(string? search, string? category);
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
    Task DeleteProductAsync(int id);
}
