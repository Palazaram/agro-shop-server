using AgroShop.Application.Abstractions;
using AgroShop.Application.RefreshTokens.CleanupExpiredRefreshTokens;

namespace AgroShop.Infrastructure.BackgroundJobs;

internal sealed class RefreshTokenCleanupJob(
    ICommandHandler<CleanupExpiredRefreshTokensCommand, int> handler)
{
    public Task RunAsync(CancellationToken ct)
        => handler.HandleAsync(new CleanupExpiredRefreshTokensCommand(), ct);
}