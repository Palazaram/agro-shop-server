using AgroShop.Domain.RefreshTokens;

namespace AgroShop.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public void Add(RefreshToken refreshToken)
        => context.RefreshTokens.Add(refreshToken);
}