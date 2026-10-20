using CSharpFunctionalExtensions;

namespace AgroShop.Domain.RefreshTokens;

public interface IRefreshTokenRepository
{
    Task<Maybe<RefreshToken>> GetByTokenHashAsync(TokenHash tokenHash, CancellationToken ct);
    Task<IReadOnlyList<RefreshToken>> GetUnrevokedByFamilyIdAsync(Guid familyId, CancellationToken ct);
    void Add(RefreshToken refreshToken);
}