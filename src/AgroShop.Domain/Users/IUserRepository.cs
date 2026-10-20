using CSharpFunctionalExtensions;

namespace AgroShop.Domain.Users;

public interface IUserRepository
{
    Task<bool> ExistsByPhoneAsync(Phone phone, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct);
    Task<Maybe<User>> GetByPhoneAsync(Phone phone, CancellationToken ct);
    Task<Maybe<User>> GetByIdAsync(Guid id, CancellationToken ct);
    void Add(User user);
}