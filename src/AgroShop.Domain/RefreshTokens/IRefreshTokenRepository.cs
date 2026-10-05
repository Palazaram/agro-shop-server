namespace AgroShop.Domain.RefreshTokens;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
}