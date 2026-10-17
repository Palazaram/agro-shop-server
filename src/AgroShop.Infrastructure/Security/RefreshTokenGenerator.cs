using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using AgroShop.Application.Abstractions;
using AgroShop.Domain.RefreshTokens;

namespace AgroShop.Infrastructure.Security;

internal sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenSizeInBytes = 32;

    public GeneratedRefreshToken Generate()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
        string value = Base64Url.EncodeToString(randomBytes);

        return new GeneratedRefreshToken(value, ComputeHash(value));
    }

    public TokenHash ComputeHash(string rawToken)
    {
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));

        return TokenHash.Create(Convert.ToHexString(hashBytes));
    }
}