using AgroShop.Domain.Users;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace AgroShop.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<bool> ExistsByPhoneAsync(Phone phone, CancellationToken ct)
        => context.Users.AnyAsync(u => u.Phone == phone, ct);

    public Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct)
        => context.Users.AnyAsync(u => u.Email == email, ct);

    public async Task<Maybe<User>> GetByPhoneAsync(Phone phone, CancellationToken ct)
        => await context.Users.FirstOrDefaultAsync(u => u.Phone == phone, ct);

    public async Task<Maybe<User>> GetByIdAsync(Guid id, CancellationToken ct)
        => await context.Users.FindAsync([id], ct);

    public void Add(User user)
        => context.Users.Add(user);
}