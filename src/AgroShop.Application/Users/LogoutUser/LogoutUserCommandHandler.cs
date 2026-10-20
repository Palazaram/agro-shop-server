using AgroShop.Application.Abstractions;
using AgroShop.Domain.Abstractions;
using AgroShop.Domain.Errors;
using AgroShop.Domain.RefreshTokens;
using CSharpFunctionalExtensions;

namespace AgroShop.Application.Users.LogoutUser;

public sealed class LogoutUserCommandHandler(
    IRefreshTokenGenerator refreshTokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    TimeProvider timeProvider,
    IUnitOfWork unitOfWork) 
    : ICommandHandler<LogoutUserCommand>
{
    public async Task<UnitResult<Error>> HandleAsync(LogoutUserCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
            return default;

        var tokenHash = refreshTokenGenerator.ComputeHash(command.RefreshToken);

        var maybeRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, ct);
        if (maybeRefreshToken.HasNoValue)
            return default;

        var refreshToken = maybeRefreshToken.Value;

        var now = timeProvider.GetUtcNow();

        refreshToken.Revoke(now);
        
        await unitOfWork.SaveChangesAsync(ct);

        return default;
    }
}