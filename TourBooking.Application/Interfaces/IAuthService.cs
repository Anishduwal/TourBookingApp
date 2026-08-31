using TourBooking.Application.DTOs;

namespace TourBooking.Application.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto?> RefreshAsync(string refreshToken);
}
