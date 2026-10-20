using AgroShop.Application.Abstractions;
using AgroShop.Domain.Users;

namespace AgroShop.Infrastructure.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public PasswordHash Hash(Password password)
        => PasswordHash.Create(BCrypt.Net.BCrypt.HashPassword(password.Value, WorkFactor));

    public bool Verify(string password, PasswordHash hash)
        => BCrypt.Net.BCrypt.Verify(password, hash.Value);
}