using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniECommerce.API.DTOs;
using MiniECommerce.API.Services;

namespace MiniECommerce.API.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    
    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    /// <summary>
    /// Get daily report of orders and shipping fees
    /// </summary>
    [HttpGet("daily")]
    public async Task<ActionResult<DailyReportDto>> GetDailyReport([FromQuery] DateTime? date)
    {
        var reportDate = date ?? DateTime.Today;
        var report = await _reportService.GetDailyReportAsync(reportDate);
        return Ok(report);
    }
}
