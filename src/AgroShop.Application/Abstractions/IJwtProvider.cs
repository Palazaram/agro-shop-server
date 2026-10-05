using AgroShop.Domain.Users;

namespace AgroShop.Application.Abstractions;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);
}