using AgroShop.Domain.Abstractions;
using AgroShop.Domain.Roles;

namespace AgroShop.Domain.Users;

public sealed class User : AggregateRoot<Guid>
{
    public Phone Phone { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public PasswordHash PasswordHash { get; private set; } = default!;
    public FullName FullName { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public Role Role { get; private set; }

    private User() { }

    private User(
        Guid id,
        Phone phone,
        Email email,
        PasswordHash passwordHash,
        FullName fullName,
        DateTimeOffset createdAt,
        Role role)
    {
        Id = id;
        Phone = phone;
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        CreatedAt = createdAt;
        Role = role;
    }

    public static User Create(
        Phone phone,
        Email email,
        PasswordHash passwordHash,
        FullName fullName,
        DateTimeOffset createdAt) 
    {
        ArgumentNullException.ThrowIfNull(phone);
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(passwordHash);
        ArgumentNullException.ThrowIfNull(fullName);

        return new(
            Guid.CreateVersion7(),
            phone,
            email,
            passwordHash,
            fullName,
            createdAt,
            Role.Customer);
    }

    public void ChangePhone(Phone phone) 
    {
        ArgumentNullException.ThrowIfNull(phone);
        Phone = phone;
    }

    public void ChangeEmail(Email email)
    {
        ArgumentNullException.ThrowIfNull(email);
        Email = email;
    }

    public void ChangeFullName(FullName fullName)
    {
        ArgumentNullException.ThrowIfNull(fullName);
        FullName = fullName;
    }

    public void ChangePassword(PasswordHash passwordHash)
    {
        ArgumentNullException.ThrowIfNull(passwordHash);
        PasswordHash = passwordHash;
    }
}