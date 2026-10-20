using AgroShop.Api.Authentication;
using AgroShop.Api.Endpoints;
using AgroShop.Api.ErrorHandling;
using AgroShop.Api.OpenApi;
using AgroShop.Api.RateLimiting;
using AgroShop.Application;
using AgroShop.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
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

    builder.Services.AddOpenApi(options =>
    {
        options.AddSchemaTransformer<RequiredFromValidatorSchemaTransformer>();
        options.AddSchemaTransformer<ProblemDetailsSchemaTransformer>();
    });

    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

    builder.Services.AddSingleton<AuthCookies>();

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.UseRateLimiter();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => options
            .WithTitle("AgroShop API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
            .SortTagsAlphabetically()
            .SortOperationsByMethod());
    }

    app.MapAuthEndpoints();

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