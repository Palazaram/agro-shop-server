using System.Diagnostics;

namespace AgroShop.Api.ErrorHandling;

internal static class ErrorHandlingExtensions
{
    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context =>
            {
                HttpContext httpContext = context.HttpContext;
                TimeProvider timeProvider = httpContext.RequestServices.GetRequiredService<TimeProvider>();

                context.ProblemDetails.Instance = httpContext.Request.Path.Value;
                context.ProblemDetails.Extensions["traceId"] =
                    Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["timestamp"] = timeProvider.GetUtcNow();
            });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}