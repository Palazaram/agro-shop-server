using AgroShop.Domain.RefreshTokens;

namespace AgroShop.Application.Abstractions;

public sealed record GeneratedRefreshToken(string Value, TokenHash Hash);
public interface IRefreshTokenGenerator
{
    GeneratedRefreshToken Generate();
    TokenHash ComputeHash(string rawToken);
}