using System.ComponentModel.DataAnnotations;

namespace TourBooking.Application.DTOs;

public class LoginRequestDto
{
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(128)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}

public class RefreshTokenRequestDto
{
    [Required, MaxLength(256)]
    public string RefreshToken { get; set; } = string.Empty;
}

public record AuthResponseDto(string AccessToken, string RefreshToken,
    DateTime AccessTokenExpiresAtUtc, DateTime RefreshTokenExpiresAtUtc);
