using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourBooking.Application.DTOs;
using TourBooking.Application.Interfaces;

namespace TourBooking.API.Controllers;

[ApiController]
[Route("api/auth")]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        if (!await authService.RegisterAsync(request))
            return Conflict(new { message = "An account with this email already exists." });
        return Ok(new { message = "User registered successfully." });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await authService.LoginAsync(request);
        return result is null ? Unauthorized(new { message = "Invalid credentials." }) : Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto request)
    {
        var result = await authService.RefreshAsync(request.RefreshToken);
        return result is null ? Unauthorized(new { message = "Invalid or expired refresh token." }) : Ok(result);
    }

    [Authorize]
    [HttpGet("profile")]
    public IActionResult Profile() => Ok(new {
        userId = User.FindFirst("uid")?.Value,
        email = User.FindFirst("sub")?.Value
    });
}
