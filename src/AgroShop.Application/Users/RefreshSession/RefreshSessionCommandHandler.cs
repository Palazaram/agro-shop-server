using AgroShop.Application.Abstractions;
using AgroShop.Application.Options;
using AgroShop.Domain.Abstractions;
using AgroShop.Domain.Errors;
using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgroShop.Application.Users.RefreshSession;

public sealed class RefreshSessionCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IRefreshTokenGenerator refreshTokenGenerator,
    IJwtProvider jwtProvider,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IOptions<RefreshTokenOptions> refreshTokenOptions,
    ILogger<RefreshSessionCommandHandler> logger)
    : ICommandHandler<RefreshSessionCommand, RefreshSessionResponse>
{
    public async Task<Result<RefreshSessionResponse, Error>> HandleAsync(RefreshSessionCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return DomainErrors.RefreshTokens.Invalid();

        var tokenHash = refreshTokenGenerator.ComputeHash(command.RefreshToken);

        var maybeRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);
        if (maybeRefreshToken.HasNoValue)
            return DomainErrors.RefreshTokens.Invalid();

        var refreshToken = maybeRefreshToken.Value;

        var maybeUser = await userRepository.GetByIdAsync(refreshToken.UserId, ct);
        if (maybeUser.HasNoValue)
            return DomainErrors.RefreshTokens.Invalid();

        var user = maybeUser.Value;

        var now = timeProvider.GetUtcNow();
        var options = refreshTokenOptions.Value;
        var generatedRefreshToken = refreshTokenGenerator.Generate();

        var rotation = refreshToken.Rotate(
            generatedRefreshToken.Hash,
            now,
            TimeSpan.FromDays(options.LifetimeDays));

        if (rotation.IsFailure)
        {
            if (rotation.Error != DomainErrors.RefreshTokens.AlreadyReplaced())
                return rotation.Error;

            if (refreshToken.WasReplacedWithin(TimeSpan.FromSeconds(options.RotationGracePeriodSeconds), now))
                return DomainErrors.RefreshTokens.ConcurrentRotation();

            await RevokeFamilyAsync(refreshToken, now, ct);
            return rotation.Error;
        }

        refreshTokenRepository.Add(rotation.Value);
        await unitOfWork.SaveChangesAsync(ct);

        var accessToken = jwtProvider.GenerateAccessToken(user);
        return new RefreshSessionResponse(accessToken, generatedRefreshToken.Value);
    }

    private async Task RevokeFamilyAsync(RefreshToken reusedToken, DateTimeOffset now, CancellationToken ct)
    {
        logger.LogWarning(
            "Refresh token reuse detected for user {UserId}, family {FamilyId}. Revoking the family",
            reusedToken.UserId, reusedToken.FamilyId);

        var familyTokens = await refreshTokenRepository.GetUnrevokedByFamilyIdAsync(reusedToken.FamilyId, ct);

        foreach (var token in familyTokens)
            token.Revoke(now);

        await unitOfWork.SaveChangesAsync(ct);
    }
}