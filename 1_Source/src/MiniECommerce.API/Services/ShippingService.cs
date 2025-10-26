using Microsoft.EntityFrameworkCore;
using MiniECommerce.API.DTOs;
using MiniECommerce.Core.Interfaces;
using MiniECommerce.Core.Models;
using MiniECommerce.Core.Strategies;
using MiniECommerce.Infrastructure.Data;

namespace MiniECommerce.API.Services;

public class ShippingService : IShippingService
{
    private readonly ApplicationDbContext _context;
    
    public ShippingService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ShippingOptionDto>> GetShippingOptionsAsync(OrderContext context)
    {
        var methods = await _context.ShippingMethods
            .Where(m => m.IsActive)
            .ToListAsync();
        
        var options = new List<ShippingOptionDto>();
        
        foreach (var method in methods)
        {
            try
            {
                var strategy = CreateStrategy(method.Code, method.ParamsJSON);
                var fee = strategy.Calculate(context);
                var details = strategy.GetCalculationDetails(context);
                
                options.Add(new ShippingOptionDto
                {
                    MethodCode = method.Code,
                    Name = method.DisplayName,
                    EstimatedFee = fee,
                    CalculationDetails = details
                });
            }
            catch (Exception ex)
            {
                // If calculation fails (e.g., cutoff time for same-day), skip this option
                options.Add(new ShippingOptionDto
                {
                    MethodCode = method.Code,
                    Name = method.DisplayName,
                    EstimatedFee = 0,
                    CalculationDetails = $"Not available: {ex.Message}"
                });
            }
        }
        
        return options;
    }
    
    public async Task<decimal> CalculateShippingFeeAsync(string methodCode, OrderContext context)
    {
        var method = await _context.ShippingMethods
            .FirstOrDefaultAsync(m => m.Code == methodCode && m.IsActive);
        
        if (method == null)
        {
            throw new InvalidOperationException($"Shipping method '{methodCode}' not found or inactive");
        }
        
        var strategy = CreateStrategy(method.Code, method.ParamsJSON);
        return strategy.Calculate(context);
    }
    
    public async Task<string> GetCalculationDetailsAsync(string methodCode, OrderContext context)
    {
        var method = await _context.ShippingMethods
            .FirstOrDefaultAsync(m => m.Code == methodCode && m.IsActive);
        
        if (method == null)
        {
            throw new InvalidOperationException($"Shipping method '{methodCode}' not found or inactive");
        }
        
        var strategy = CreateStrategy(method.Code, method.ParamsJSON);
        return strategy.GetCalculationDetails(context);
    }
    
    public async Task<List<ShippingMethodDto>> GetAllMethodsAsync()
    {
        var methods = await _context.ShippingMethods.ToListAsync();
        
        return methods.Select(m => new ShippingMethodDto
        {
            Id = m.Id,
            Code = m.Code,
            DisplayName = m.DisplayName,
            ParamsJSON = m.ParamsJSON,
            IsActive = m.IsActive
        }).ToList();
    }
    
    public async Task<ShippingMethodDto> UpdateMethodAsync(int id, UpdateShippingMethodDto dto)
    {
        var method = await _context.ShippingMethods.FindAsync(id);
        
        if (method == null)
        {
            throw new InvalidOperationException("Shipping method not found");
        }
        
        method.DisplayName = dto.DisplayName;
        method.ParamsJSON = dto.ParamsJSON;
        method.IsActive = dto.IsActive;
        method.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return new ShippingMethodDto
        {
            Id = method.Id,
            Code = method.Code,
            DisplayName = method.DisplayName,
            ParamsJSON = method.ParamsJSON,
            IsActive = method.IsActive
        };
    }
    
    private IShippingStrategy CreateStrategy(string code, string paramsJson)
    {
        return code.ToUpper() switch
        {
            "STANDARD" => new StandardShippingStrategy(paramsJson),
            "EXPRESS" => new ExpressShippingStrategy(paramsJson),
            "SAME_DAY" => new SameDayShippingStrategy(paramsJson),
            "ECO" => new EcoShippingStrategy(paramsJson),
            _ => throw new InvalidOperationException($"Unknown shipping strategy: {code}")
        };
    }
}
