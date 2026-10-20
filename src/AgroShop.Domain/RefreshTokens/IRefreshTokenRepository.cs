using CSharpFunctionalExtensions;

namespace AgroShop.Domain.RefreshTokens;

public interface IRefreshTokenRepository
{
    Task<Maybe<RefreshToken>> GetByTokenHashAsync(TokenHash tokenHash, CancellationToken ct);
    void Add(RefreshToken refreshToken);
}