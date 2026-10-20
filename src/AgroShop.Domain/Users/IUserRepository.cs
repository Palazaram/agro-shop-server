using CSharpFunctionalExtensions;

namespace AgroShop.Domain.Users;

public interface IUserRepository
{
    Task<bool> ExistsByPhoneAsync(Phone phone, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct);
    Task<Maybe<User>> GetByPhoneAsync(Phone phone, CancellationToken ct);
    void Add(User user);
}