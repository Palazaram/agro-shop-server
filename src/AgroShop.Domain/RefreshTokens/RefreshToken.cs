using AgroShop.Domain.Abstractions;
using AgroShop.Domain.Errors;
using CSharpFunctionalExtensions;

namespace AgroShop.Domain.RefreshTokens;

public sealed class RefreshToken : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public Guid FamilyId { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
    public TokenHash TokenHash { get; private set; } = default!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt is not null;

    private RefreshToken() { }

    private RefreshToken(
        Guid id,
        Guid userId,
        Guid familyId,
        TokenHash tokenHash,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        Id = id;
        UserId = userId;
        FamilyId = familyId;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(
        Guid userId,
        TokenHash tokenHash,
        DateTimeOffset createdAt,
        TimeSpan lifetime) 
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User id must not be empty", nameof(userId));

        return Issue(userId, Guid.CreateVersion7(), tokenHash, createdAt, lifetime);
    }

    public bool IsExpired(DateTimeOffset now) => ExpiresAt <= now;
    public bool IsActive(DateTimeOffset now) => !IsRevoked && !IsExpired(now);

    public void Revoke(DateTimeOffset revokedAt) 
    {
        if (IsRevoked)
            return;

        RevokedAt = revokedAt;
    }

    public Result<RefreshToken, Error> Rotate(TokenHash newHash, DateTimeOffset now, TimeSpan lifetime) 
    {
        if (ReplacedByTokenId is not null)
            return DomainErrors.RefreshTokens.AlreadyReplaced();

        if (!IsActive(now))
            return DomainErrors.RefreshTokens.Invalid();

        var newToken = Issue(UserId, FamilyId, newHash, now, lifetime);

        ReplacedByTokenId = newToken.Id;
        Revoke(now);

        return newToken;
    }

    private static RefreshToken Issue(
        Guid userId,
        Guid familyId,
        TokenHash tokenHash,
        DateTimeOffset createdAt,
        TimeSpan lifetime)
    {
        ArgumentNullException.ThrowIfNull(tokenHash);

        if (lifetime <= TimeSpan.Zero)
            throw new ArgumentException("Lifetime must be greater than zero", nameof(lifetime));

        return new(
            Guid.CreateVersion7(),
            userId,
            familyId,
            tokenHash,
            createdAt,
            createdAt + lifetime);
    }
}