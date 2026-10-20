using AgroShop.Application.Options;
using AgroShop.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace AgroShop.Api.Authentication;

internal sealed class AuthCookies(
    IOptions<JwtOptions> jwtOptions,
    IOptions<RefreshTokenOptions> refreshTokenOptions)
{
    public const string AccessTokenName = "access_token";
    public const string RefreshTokenName = "refresh_token";

    private const string AccessTokenPath = "/";
    private const string RefreshTokenPath = "/api/auth";

    public void SetTokens(HttpResponse response, string accessToken, string refreshToken)
    {
        response.Cookies.Append(
            AccessTokenName,
            accessToken,
            CreateOptions(AccessTokenPath, TimeSpan.FromMinutes(jwtOptions.Value.AccessTokenExpirationMinutes)));

        response.Cookies.Append(
            RefreshTokenName,
            refreshToken,
            CreateOptions(RefreshTokenPath, TimeSpan.FromDays(refreshTokenOptions.Value.LifetimeDays)));
    }

    public void ClearTokens(HttpResponse response)
    {
        response.Cookies.Delete(AccessTokenName, CreateOptions(AccessTokenPath));
        response.Cookies.Delete(RefreshTokenName, CreateOptions(RefreshTokenPath));
    }

    private static CookieOptions CreateOptions(string path, TimeSpan? maxAge = null) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        Path = path,
        MaxAge = maxAge
    };
}