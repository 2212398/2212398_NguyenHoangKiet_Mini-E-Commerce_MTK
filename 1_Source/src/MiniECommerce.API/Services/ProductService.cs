using Microsoft.EntityFrameworkCore;
using MiniECommerce.API.DTOs;
using MiniECommerce.Core.Entities;
using MiniECommerce.Infrastructure.Data;

namespace MiniECommerce.API.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    
    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ProductDto>> GetProductsAsync(string? search, string? category)
    {
        var query = _context.Products.Where(p => p.IsActive);
        
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Description!.Contains(search));
        }
        
        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(p => p.Category == category);
        }
        
        var products = await query.ToListAsync();
        
        return products.Select(MapToDto).ToList();
    }
    
    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null || !product.IsActive)
        {
            throw new InvalidOperationException("Product not found");
        }
        
        return MapToDto(product);
    }
    
    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Category = dto.Category,
            Price = dto.Price,
            Weight = dto.Weight,
            Stock = dto.Stock,
            Description = dto.Description,
            IsActive = true
        };
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return MapToDto(product);
    }
    
    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }
        
        product.Name = dto.Name;
        product.Category = dto.Category;
        product.Price = dto.Price;
        product.Weight = dto.Weight;
        product.Stock = dto.Stock;
        product.Description = dto.Description;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return MapToDto(product);
    }
    
    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        
        if (product == null)
        {
            throw new InvalidOperationException("Product not found");
        }
        
        // Soft delete
        product.IsActive = false;
        await _context.SaveChangesAsync();
    }
    
    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Weight = product.Weight,
            Stock = product.Stock,
            Description = product.Description ?? "",
            IsActive = product.IsActive
        };
    }
}
