using System.Text.RegularExpressions;
using AgroShop.Domain.Errors;
using CSharpFunctionalExtensions;

namespace AgroShop.Domain.Users;

public sealed partial class Password : SimpleValueObject<string>
{
    public const int MinLength = 8;
    public const int MaxLength = 64;

    [GeneratedRegex(@"^[!-~]+\z")]
    private static partial Regex AllowedCharacters();

    private Password(string value) : base(value) { }

    public static Result<Password, Error> Create(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return DomainErrors.Users.PasswordIsRequired();

        if (value.Length > MaxLength)
            return DomainErrors.Users.PasswordTooLong(MaxLength);

        if (!AllowedCharacters().IsMatch(value))
            return DomainErrors.Users.PasswordHasUnsupportedCharacters();

        if (value.Length < MinLength)
            return DomainErrors.Users.PasswordTooShort(MinLength);

        return new Password(value);
    }

    public override string ToString() => "***";
}