namespace TourBooking.Infrastructure.Services;

public class JwtOptions
{
    public string Issuer { get; set; } = "TourBooking";
    public string Audience { get; set; } = "TourBooking.Client";
    public string Key { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}
