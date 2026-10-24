using AgroShop.Api.Authentication;
using AgroShop.Api.Endpoints;
using AgroShop.Api.ErrorHandling;
using AgroShop.Api.OpenApi;
using AgroShop.Api.RateLimiting;
using AgroShop.Application;
using AgroShop.Infrastructure;
using AgroShop.Infrastructure.BackgroundJobs;
using Hangfire;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    });

    builder.Services.AddSerilog((services, configuration) => configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration);

    builder.Services.AddErrorHandling();
    
    builder.Services.AddApiRateLimiting();

    builder.Services.AddJwtAuthentication();

    builder.Services.AddOpenApi(options =>
    {
        options.AddSchemaTransformer<RequiredFromValidatorSchemaTransformer>();
        options.AddSchemaTransformer<ProblemDetailsSchemaTransformer>();
    });

    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict;
        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
    });

    builder.Services.AddSingleton<AuthCookies>();

    var app = builder.Build();

    app.Services.ScheduleRecurringJobs();

    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, exception) =>
            exception is not null || httpContext.Response.StatusCode >= 500
                ? LogEventLevel.Error
                : httpContext.Request.Path.StartsWithSegments("/hangfire")
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information;
    });

    app.UseExceptionHandler();
    app.UseRateLimiter();
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => options
            .WithTitle("AgroShop API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
            .SortTagsAlphabetically()
            .SortOperationsByMethod());
        app.MapHangfireDashboard();
    }

    app.MapAuthEndpoints();
    app.MapUserEndpoints();

    app.Run();
}
catch (Exception exception) when (exception is not HostAbortedException)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
    Environment.ExitCode = 1;
}
finally
{
    Log.CloseAndFlush();
}