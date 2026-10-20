using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AgroShop.Api.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        string? subject = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(subject, out Guid userId))
            throw new InvalidOperationException("Authenticated principal has no valid 'sub' claim.");

        return userId;
    }
}