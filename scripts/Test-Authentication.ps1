# Requires PowerShell 7 and an already-running API with migrations applied.
# Creates one test account; run only against a development/test database.
param([Parameter(Mandatory)][string]$BaseUrl)
$ErrorActionPreference = 'Stop'
$BaseUrl = $BaseUrl.TrimEnd('/')
$email = "auth-test-$([Guid]::NewGuid().ToString('N'))@example.com"
$password = [Guid]::NewGuid().ToString('N') + '!aA1'

function Invoke-Check([string]$Path, [string]$Method, $Body, [int]$Expected, [string]$Token = '') {
    $request = @{
        Uri = "$BaseUrl/api/auth/$Path"
        Method = $Method
        ContentType = 'application/json'
        SkipHttpErrorCheck = $true
    }
    if ($null -ne $Body) { $request.Body = $Body | ConvertTo-Json -Compress }
    if ($Token) { $request.Headers = @{ Authorization = "Bearer $Token" } }
    $response = Invoke-WebRequest @request
    if ([int]$response.StatusCode -ne $Expected) {
        throw "$Method $Path returned $($response.StatusCode); expected $Expected."
    }
    if ($response.Content) { return $response.Content | ConvertFrom-Json }
}

$null = Invoke-Check 'profile' 'GET' $null 401
$null = Invoke-Check 'profile' 'GET' $null 401 'invalid-token'
$null = Invoke-Check 'register' 'POST' @{email='invalid';password='short'} 400
$null = Invoke-Check 'register' 'POST' @{email=$email;password=$password} 200
$null = Invoke-Check 'register' 'POST' @{email=$email.ToUpperInvariant();password=$password} 409
$null = Invoke-Check 'login' 'POST' @{email=$email;password='wrong-password'} 401
$tokens = Invoke-Check 'login' 'POST' @{email=$email.ToUpperInvariant();password=$password} 200
$profile = Invoke-Check 'profile' 'GET' $null 200 $tokens.accessToken
if ($profile.email -ne $email -or !$profile.userId) { throw 'Profile claims are incorrect.' }
$null = Invoke-Check 'refresh' 'POST' @{refreshToken='invalid-token'} 401
$rotated = Invoke-Check 'refresh' 'POST' @{refreshToken=$tokens.refreshToken} 200
if ($rotated.refreshToken -eq $tokens.refreshToken) { throw 'Refresh token did not rotate.' }
$null = Invoke-Check 'refresh' 'POST' @{refreshToken=$tokens.refreshToken} 401
$null = Invoke-Check 'profile' 'GET' $null 200 $rotated.accessToken
Write-Output 'Authentication checks passed (registration, validation, login, JWT, refresh, and replay rejection).'
