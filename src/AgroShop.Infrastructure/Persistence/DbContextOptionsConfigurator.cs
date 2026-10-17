using Microsoft.EntityFrameworkCore;

namespace AgroShop.Infrastructure.Persistence;

internal static class DbContextOptionsConfigurator
{
    public static void Configure(DbContextOptionsBuilder builder, string connectionString)
        => builder
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention();
}