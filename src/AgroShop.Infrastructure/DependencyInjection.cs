using AgroShop.Application.Abstractions;
using AgroShop.Application.Extensions;
using AgroShop.Domain.Abstractions;
using AgroShop.Domain.RefreshTokens;
using AgroShop.Domain.Users;
using AgroShop.Infrastructure.Options;
using AgroShop.Infrastructure.Persistence;
using AgroShop.Infrastructure.Persistence.Repositories;
using AgroShop.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AgroShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddValidatedOptions<JwtOptions>(JwtOptions.SectionName);

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Connection string 'Database' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            DbContextOptionsConfigurator.Configure(options, connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }
}