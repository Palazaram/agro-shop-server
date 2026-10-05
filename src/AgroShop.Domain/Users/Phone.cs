using AgroShop.Domain.Errors;
using CSharpFunctionalExtensions;
using System.Text.RegularExpressions;

namespace AgroShop.Domain.Users;

public sealed partial class Phone : SimpleValueObject<string>
{
    public const int NationalNumberLength = 9;  // 99 628 66 44
    public const int MaxLength = 13;            // +380 + 9 цифр

    private const string CountryCode = "380";
    private const string CountryPrefix = "+380";
    private const int OperatorCodeLength = 2;

    [GeneratedRegex(@"^\+?[0-9\s()-]+$")]
    private static partial Regex AllowedCharacters();

    [GeneratedRegex(@"[^0-9]")]
    private static partial Regex NonDigits();

    private Phone(string value) : base(value) { }

    public static Result<Phone, Error> Create(string? value) 
    {
        if (string.IsNullOrWhiteSpace(value))
            return DomainErrors.Users.PhoneIsRequired();

        var normalized = value.Trim();

        if (!AllowedCharacters().IsMatch(normalized))
            return DomainErrors.Users.PhoneInvalidFormat();

        string digits = NonDigits().Replace(normalized, string.Empty);

        string? national = ExtractNationalNumber(digits);

        if (national is null)
            return DomainErrors.Users.PhoneInvalidLength(NationalNumberLength);

        if (!IsAllocatedMobileCode(national.AsSpan(0, OperatorCodeLength)))
            return DomainErrors.Users.PhoneUnknownOperatorCode();

        return new Phone(CountryPrefix + national);
    }

    private static string? ExtractNationalNumber(string digits) => digits.Length switch
    {
        NationalNumberLength => digits,
        10 when digits[0] == '0' => digits[1..],
        12 when digits.StartsWith(CountryCode, StringComparison.Ordinal) => digits[CountryCode.Length..],
        _ => null
    };

    private static bool IsAllocatedMobileCode(ReadOnlySpan<char> code) => code switch
    {
        "39" or "50" or "63" or "66" or "67" or "68" or "73" or "91"
            or "92" or "93" or "94" or "95" or "96" or "97" or "98" or "99" => true,
        _ => false
    };
}
