using System.Text.RegularExpressions;
using AgroShop.Domain.Errors;
using CSharpFunctionalExtensions;

namespace AgroShop.Domain.Users;

public sealed partial class PersonName : SimpleValueObject<string>
{
    public const int MaxLength = 60;
    private const int MinLength = 2;

    private const string Letters = "АБВГҐДЕЄЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЬЮЯабвгґдеєжзиіїйклмнопрстуфхцчшщьюя";

    [GeneratedRegex("^[" + Letters + "]+(?:['\\- ][" + Letters + "]+)*$")]
    private static partial Regex Format();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRuns();

    private PersonName(string value) : base(value) { }

    public static Result<PersonName, Error> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DomainErrors.Users.NameIsRequired();

        string normalized = WhitespaceRuns()
            .Replace(value.Trim(), " ")
            .Replace('\u2019', '\'')   // ’ — типографский, его ставит автозамена
            .Replace('\u02BC', '\'')   // ʼ — украинский апостроф
            .Replace('\u2018', '\'')   // ‘
            .Replace('`', '\'');       // ` — с клавиши под Esc, так часто пишут

        if (normalized.Length > MaxLength)
            return DomainErrors.Users.NameTooLong(MaxLength);

        if (!Format().IsMatch(normalized))
            return DomainErrors.Users.NameInvalidFormat();

        if (normalized.Length < MinLength)
            return DomainErrors.Users.NameTooShort(MinLength);

        return new PersonName(normalized);
    }
}