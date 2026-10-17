using AgroShop.Application.Abstractions;
using AgroShop.Application.Extensions;
using AgroShop.Infrastructure.Options;
using AgroShop.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace AgroShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddValidatedOptions<JwtOptions>(JwtOptions.SectionName);

        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IJwtProvider, JwtProvider>();
        
        return services;
    }
}