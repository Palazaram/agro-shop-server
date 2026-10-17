using System.Security.Claims;
using System.Text;
using AgroShop.Application.Abstractions;
using AgroShop.Domain.Users;
using AgroShop.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AgroShop.Infrastructure.Security;

internal sealed class JwtProvider(IOptions<JwtOptions> options, TimeProvider timeProvider) : IJwtProvider
{
    private static readonly JsonWebTokenHandler TokenHandler = new();

    public string GenerateAccessToken(User user)
    {
        JwtOptions jwt = options.Value;
        DateTime now = timeProvider.GetUtcNow().UtcDateTime;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimNames.Role, user.Role.ToString())
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = now,
            NotBefore = now,
            Expires = now.AddMinutes(jwt.AccessTokenExpirationMinutes),
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        return TokenHandler.CreateToken(descriptor);
    }
}