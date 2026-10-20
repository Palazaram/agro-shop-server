using AgroShop.Application.Abstractions;
using AgroShop.Application.Extensions;
using AgroShop.Application.Options;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AgroShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddSingleton(TimeProvider.System);
        services.AddValidatorsFromAssembly(assembly);
        services.AddValidatedOptions<RefreshTokenOptions>(RefreshTokenOptions.SectionName);

        AddHandlers(services, assembly);

        return services;
    }

    private static void AddHandlers(IServiceCollection services, Assembly assembly)
    {
        services.Scan(scan => scan
            .FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableToAny(typeof(ICommandHandler<>), typeof(ICommandHandler<,>))
                .Where(type => !type.IsGenericTypeDefinition))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}