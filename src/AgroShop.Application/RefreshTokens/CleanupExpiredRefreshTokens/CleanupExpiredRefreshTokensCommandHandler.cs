using AgroShop.Application.Abstractions;
using AgroShop.Application.Options;
using AgroShop.Domain.Errors;
using AgroShop.Domain.RefreshTokens;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgroShop.Application.RefreshTokens.CleanupExpiredRefreshTokens;

public sealed class CleanupExpiredRefreshTokensCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    TimeProvider timeProvider,
    IOptions<RefreshTokenOptions> options,
    ILogger<CleanupExpiredRefreshTokensCommandHandler> logger)
    : ICommandHandler<CleanupExpiredRefreshTokensCommand, int>
{
    public async Task<Result<int, Error>> HandleAsync(CleanupExpiredRefreshTokensCommand command, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow();
        var cutoff = now.AddDays(-options.Value.RetentionDaysAfterExpiry);
        var deletedRows = await refreshTokenRepository.DeleteExpiredBeforeAsync(cutoff, ct);
        logger.LogInformation("Deleted {DeletedRows} expired refresh tokens (expired before {Cutoff})", deletedRows, cutoff);
        return deletedRows;
    }
}