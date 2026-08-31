Hello everyone, this is a tour booking app to book ride as where you want inside some cities. This app is created with clean architecture using ASP.NET Core 8.0.

## Authentication

TourBooking now contains its own authentication flow adapted from the separate Login project.
It uses TourBooking's database; existing Login accounts and tokens are not imported.

Set these environment variables before starting the API (PowerShell):

```powershell
# Generate once and store securely; keep the same key across restarts/instances.
$env:Jwt__Key = [Convert]::ToBase64String([System.Security.Cryptography.RandomNumberGenerator]::GetBytes(48))
$env:Jwt__Issuer = 'TourBooking'
$env:Jwt__Audience = 'TourBooking.Client'
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=TourBookingDb;Trusted_Connection=True;TrustServerCertificate=True'
dotnet ef database update --project TourBooking.Infrastructure --startup-project TourBooking.API
dotnet run --project TourBooking.API
```

Replace the database connection with your intended SQL Server before running the migration.
The EF design-time factory uses `ConnectionStrings__DefaultConnection`, falling back to
LocalDB/TourBookingDb; it does not read the API appsettings connection. Runtime uses normal
ASP.NET Core configuration. No migration is automatically applied at application startup.
Never commit a signing key. Missing keys or keys shorter than 32 UTF-8 bytes prevent startup.
Defaults are 15 minutes for access tokens and 7 days for refresh tokens; override with
`Jwt__AccessTokenMinutes` and `Jwt__RefreshTokenDays`.

| Endpoint | Body / authorization | Result |
| --- | --- | --- |
| `POST /api/auth/register` | `{"email":"you@example.com","password":"a-long-unique-password"}` | 200; duplicate email returns 409 |
| `POST /api/auth/login` | Same email/password fields | Access token, refresh token, and UTC expiry timestamps; invalid credentials return 401 |
| `POST /api/auth/refresh` | `{"refreshToken":"<refresh token>"}` | New token pair; expired/reused tokens return 401 |
| `GET /api/auth/profile` | `Authorization: Bearer <access token>` | User ID and email; missing/invalid token returns 401 |

Registration requires a valid email and a password of 8–128 characters. Invalid request
bodies return 400. Emails are trimmed and matched case-insensitively. Passwords use the
ASP.NET Core password hasher. Refresh tokens are stored as SHA-256 hashes and rotated
atomically with SQL Server rowversion concurrency protection. Clients must replace both
tokens after refresh. Use HTTPS and keep tokens out of logs and URLs.

Existing tour and booking endpoints retain their original access rules. Only the new
profile endpoint requires authentication; add suitable authorization/ownership policies
before exposing private booking data. Add login throttling before public deployment.

Run `pwsh -File scripts/Test-Authentication.ps1 -BaseUrl https://localhost:<port>`
against a running development instance to check registration, duplicate detection,
invalid credentials, protected profile access, and refresh-token replay rejection.
This creates one test account. Use a trusted development HTTPS certificate.
