using System.Globalization;
using System.Threading.RateLimiting;
using AgroShop.Api.ErrorHandling;
using Microsoft.AspNetCore.RateLimiting;

namespace AgroShop.Api.RateLimiting;

internal static class RateLimitingExtensions
{
    public const string LoginPolicy = "login";
    public const string RegisterPolicy = "register";

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(LoginPolicy, httpContext => CreatePerIpLimiter(httpContext, permitLimit: 15));
            options.AddPolicy(RegisterPolicy, httpContext => CreatePerIpLimiter(httpContext, permitLimit: 15));

            options.OnRejected = WriteRejectionAsync;
        });

        return services;
    }

    private static async ValueTask WriteRejectionAsync(OnRejectedContext context, CancellationToken cancellationToken)
    {
        HttpContext httpContext = context.HttpContext;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            httpContext.Response.Headers.RetryAfter =
                ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
        }

        httpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("AgroShop.Api.RateLimiting")
            .LogWarning("Rate limit exceeded for {Method} {Path} from {ClientIp}",
                httpContext.Request.Method, httpContext.Request.Path, httpContext.Connection.RemoteIpAddress);

        var problemDetails = ApiProblemDetails.Create(
            StatusCodes.Status429TooManyRequests,
            "Too Many Requests",
            new ApiError(null, "request.rate_limited", "Забагато спроб. Спробуйте за хвилину"));

        problemDetails.Type = "https://tools.ietf.org/html/rfc6585#section-4";

        await httpContext.RequestServices
            .GetRequiredService<IProblemDetailsService>()
            .TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }

    private static RateLimitPartition<string> CreatePerIpLimiter(HttpContext httpContext, int permitLimit)
        => RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0
            });
}