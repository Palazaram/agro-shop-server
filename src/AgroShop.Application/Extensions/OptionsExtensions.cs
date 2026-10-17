using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AgroShop.Application.Extensions;

public static class OptionsExtensions
{
    public static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>(
        this IServiceCollection services, string sectionName)
        where TOptions : class
        => services.AddOptions<TOptions>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
}