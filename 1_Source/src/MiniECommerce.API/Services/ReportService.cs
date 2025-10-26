using Microsoft.EntityFrameworkCore;
using MiniECommerce.API.DTOs;
using MiniECommerce.Infrastructure.Data;

namespace MiniECommerce.API.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    
    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<DailyReportDto> GetDailyReportAsync(DateTime date)
    {
        var startDate = date.Date;
        var endDate = startDate.AddDays(1);
        
        var orders = await _context.Orders
            .Where(o => o.CreatedAt >= startDate && o.CreatedAt < endDate)
            .ToListAsync();
        
        var report = new DailyReportDto
        {
            Date = date.Date,
            TotalOrders = orders.Count,
            TotalRevenue = orders.Sum(o => o.GrandTotal),
            TotalShippingFees = orders.Sum(o => o.ShippingFee),
            AverageOrderValue = orders.Any() ? orders.Average(o => o.GrandTotal) : 0,
            OrdersByMethod = orders
                .GroupBy(o => o.MethodCode)
                .Select(g => new ShippingMethodStats
                {
                    MethodCode = g.Key,
                    OrderCount = g.Count(),
                    TotalFees = g.Sum(o => o.ShippingFee)
                })
                .ToList()
        };
        
        return report;
    }
}
