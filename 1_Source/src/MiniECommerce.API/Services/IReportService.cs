using MiniECommerce.API.DTOs;

namespace MiniECommerce.API.Services;

public interface IReportService
{
    Task<DailyReportDto> GetDailyReportAsync(DateTime date);
}
