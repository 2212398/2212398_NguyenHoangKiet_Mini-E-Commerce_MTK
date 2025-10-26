using MiniECommerce.API.DTOs;

namespace MiniECommerce.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<int> GetCurrentUserIdAsync(string email);
}
