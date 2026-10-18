namespace AgroShop.Infrastructure.Persistence;

public sealed class DuplicateKeyException(string? constraintName, Exception innerException)
    : Exception($"Unique constraint '{constraintName}' was violated.", innerException)
{
    public string? ConstraintName { get; } = constraintName;
}