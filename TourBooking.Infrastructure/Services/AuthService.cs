using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TourBooking.Application.DTOs;
using TourBooking.Application.Interfaces;
using TourBooking.Domain.Entities;

namespace TourBooking.Infrastructure.Services;

public class AuthService(ApplicationDbContext db, IOptions<JwtOptions> options,
    IPasswordHasher<User> passwordHasher) : IAuthService
{
    private readonly JwtOptions jwt = options.Value;

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        var email = request.Email.Trim();
        var normalized = email.ToUpperInvariant();
        if (await db.Users.AnyAsync(u => u.NormalizedEmail == normalized)) return false;

        var user = new User { Email = email, NormalizedEmail = normalized };
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);
        db.Users.Add(user);
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            db.Entry(user).State = EntityState.Detached;
            return false;
        }
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var normalized = request.Email.Trim().ToUpperInvariant();
        var user = await db.Users.SingleOrDefaultAsync(u => u.NormalizedEmail == normalized);
        if (user is null) return null;
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed) return null;
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        var response = IssueTokens(user);
        await db.SaveChangesAsync();
        return response;
    }

    public async Task<AuthResponseDto?> RefreshAsync(string refreshToken)
    {
        var hash = HashToken(refreshToken);
        var stored = await db.RefreshTokens.Include(t => t.User)
            .SingleOrDefaultAsync(t => t.TokenHash == hash);
        if (stored is null || stored.RevokedAtUtc is not null || stored.ExpiresAtUtc <= DateTime.UtcNow)
            return null;

        stored.RevokedAtUtc = DateTime.UtcNow;
        var response = IssueTokens(stored.User);
        try
        {
            // A single SaveChanges transaction and rowversion prevent concurrent reuse.
            await db.SaveChangesAsync();
            return response;
        }
        catch (DbUpdateConcurrencyException)
        {
            db.ChangeTracker.Clear();
            return null;
        }
    }

    private AuthResponseDto IssueTokens(User user)
    {
        var now = DateTime.UtcNow;
        var accessExpiry = now.AddMinutes(jwt.AccessTokenMinutes);
        var refreshExpiry = now.AddDays(jwt.RefreshTokenDays);
        var token = new JwtSecurityToken(jwt.Issuer, jwt.Audience,
            new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("uid", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }, notBefore: now, expires: accessExpiry,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)), SecurityAlgorithms.HmacSha256));
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        db.RefreshTokens.Add(new RefreshToken {
            UserId = user.Id, TokenHash = HashToken(refresh), ExpiresAtUtc = refreshExpiry
        });
        return new(new JwtSecurityTokenHandler().WriteToken(token), refresh, accessExpiry, refreshExpiry);
    }

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
