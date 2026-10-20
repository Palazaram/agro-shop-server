using AgroShop.Domain.RefreshTokens;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace AgroShop.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<Maybe<RefreshToken>> GetByTokenHashAsync(TokenHash tokenHash, CancellationToken ct)
        => await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

    public async Task<IReadOnlyList<RefreshToken>> GetUnrevokedByFamilyIdAsync(Guid familyId, CancellationToken ct)
        => await context.RefreshTokens
            .Where(t => t.FamilyId == familyId && t.RevokedAt == null)
            .ToListAsync(ct);

    public void Add(RefreshToken refreshToken)
        => context.RefreshTokens.Add(refreshToken);
}