using AgroShop.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace AgroShop.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<bool> ExistsByPhoneAsync(Phone phone, CancellationToken ct)
        => context.Users.AnyAsync(u => u.Phone == phone, ct);

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct)
        => context.Users.AnyAsync(u => u.Email == email, ct);

    public void Add(User user)
        => context.Users.Add(user);
}