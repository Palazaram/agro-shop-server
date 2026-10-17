using System.ComponentModel.DataAnnotations;

namespace AgroShop.Application.Options;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    [Range(1, 365)]
    public int LifetimeDays { get; init; }

    [Range(1, 365)]
    public int RetentionDaysAfterExpiry { get; init; }
}