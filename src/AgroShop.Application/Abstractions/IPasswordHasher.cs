using AgroShop.Domain.Users;

namespace AgroShop.Application.Abstractions;

public interface IPasswordHasher
{
    PasswordHash Hash(string password);
}