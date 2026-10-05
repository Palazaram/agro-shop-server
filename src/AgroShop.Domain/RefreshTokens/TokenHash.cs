using CSharpFunctionalExtensions;

namespace AgroShop.Domain.RefreshTokens;

public sealed class TokenHash : SimpleValueObject<string>
{
    private TokenHash(string value) : base(value) { }

    public static TokenHash Create(string value) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new TokenHash(value);
    }
}