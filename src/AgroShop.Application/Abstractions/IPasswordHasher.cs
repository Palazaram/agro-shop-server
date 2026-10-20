using AgroShop.Domain.Users;

namespace AgroShop.Application.Abstractions;

public interface IPasswordHasher
{
    PasswordHash Hash(Password password);
    bool Verify(string password, PasswordHash hash);
}