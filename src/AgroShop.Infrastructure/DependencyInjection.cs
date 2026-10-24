using AgroShop.Application.Abstractions;
using AgroShop.Application.Extensions;
using AgroShop.Application.Users;
using AgroShop.Domain.Abstractions;
using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using AgroShop.Infrastructure.Options;
using AgroShop.Infrastructure.Persistence;
using AgroShop.Infrastructure.Persistence.Queries;
using AgroShop.Infrastructure.Persistence.Repositories;
using AgroShop.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.PostgreSql;

namespace AgroShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddValidatedOptions<JwtOptions>(JwtOptions.SectionName);

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        AddPersistence(services, connectionString);
        AddBackgroundJobs(services, connectionString);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            DbContextOptionsConfigurator.Configure(options, connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUserQueries, UserQueries>();
    }

    private static void AddBackgroundJobs(IServiceCollection services, string connectionString)
    {
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();
    }
}