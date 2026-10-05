using CSharpFunctionalExtensions;

namespace AgroShop.Domain.Users;

public sealed class PasswordHash : SimpleValueObject<string>
{
    private PasswordHash(string value) : base(value) { }

    public static PasswordHash Create(string value) 
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new PasswordHash(value);
    }
}