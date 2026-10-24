using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace AgroShop.Infrastructure.BackgroundJobs;

public static class RecurringJobScheduling
{
    private static readonly TimeZoneInfo KyivTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");

    public static void ScheduleRecurringJobs(this IServiceProvider services)
    {
        var recurringJobs = services.GetRequiredService<IRecurringJobManager>();

        recurringJobs.AddOrUpdate<RefreshTokenCleanupJob>(
            "refresh-token-cleanup",
            job => job.RunAsync(CancellationToken.None),
            Cron.Daily(3),
            new RecurringJobOptions { TimeZone = KyivTimeZone });
    }
}