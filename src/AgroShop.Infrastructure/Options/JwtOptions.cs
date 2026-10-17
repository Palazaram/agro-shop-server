using System.ComponentModel.DataAnnotations;

namespace AgroShop.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = default!;

    [Required]
    public string Audience { get; init; } = default!;

    [Required, MinLength(32)]
    public string SecretKey { get; init; } = default!;

    [Range(1, 60)]
    public int AccessTokenExpirationMinutes { get; init; }
}