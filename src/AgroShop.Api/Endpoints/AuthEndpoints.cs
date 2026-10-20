using AgroShop.Api.Authentication;
using AgroShop.Api.ErrorHandling;
using AgroShop.Api.RateLimiting;
using AgroShop.Api.Validation;
using AgroShop.Application.Abstractions;
using AgroShop.Application.Users.LoginUser;
using AgroShop.Application.Users.LogoutUser;
using AgroShop.Application.Users.RefreshSession;
using AgroShop.Application.Users.RegisterUser;
using AgroShop.Domain.Errors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AgroShop.Api.Endpoints;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", RegisterAsync)
            .WithName("RegisterUser")
            .WithSummary("Register a new customer and sign them in")
            .WithRequestValidation<RegisterUserCommand>()
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting(RateLimitingExtensions.RegisterPolicy);

        group.MapPost("/login", LoginAsync)
            .WithName("LoginUser")
            .WithSummary("Login an existing user")
            .WithRequestValidation<LoginUserCommand>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting(RateLimitingExtensions.LoginPolicy);

        group.MapPost("/logout", LogoutAsync)
            .WithName("LogoutUser")
            .WithSummary("Logout the current user");

        group.MapPost("/refresh", RefreshAsync)
            .WithName("RefreshSession")
            .WithSummary("Rotate the refresh token and issue a new access token")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }

    private static async Task<Results<Created, ProblemHttpResult>> RegisterAsync(
        RegisterUserCommand command,
        ICommandHandler<RegisterUserCommand, RegisterUserResponse> handler,
        AuthCookies authCookies,
        HttpResponse response,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        if (result.IsFailure)
            return result.Error.ToProblem();

        authCookies.SetTokens(response, result.Value.AccessToken, result.Value.RefreshToken);

        return TypedResults.Created();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> LoginAsync(
        LoginUserCommand command,
        ICommandHandler<LoginUserCommand, LoginUserResponse> handler,
        AuthCookies authCookies,
        HttpResponse response,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        if (result.IsFailure)
            return result.Error.ToProblem();

        authCookies.SetTokens(response, result.Value.AccessToken, result.Value.RefreshToken);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> LogoutAsync(
        HttpRequest request,
        ICommandHandler<LogoutUserCommand> handler,
        AuthCookies authCookies,
        HttpResponse response,
        CancellationToken ct)
    {
        var refreshToken = request.Cookies[AuthCookies.RefreshTokenName];
        var command = new LogoutUserCommand(refreshToken);
        var result = await handler.HandleAsync(command, ct);

        if (result.IsFailure)
            return result.Error.ToProblem();

        authCookies.ClearTokens(response);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> RefreshAsync(
        HttpRequest request,
        ICommandHandler<RefreshSessionCommand, RefreshSessionResponse> handler,
        AuthCookies authCookies,
        HttpResponse response,
        CancellationToken ct)
    {
        var command = new RefreshSessionCommand(request.Cookies[AuthCookies.RefreshTokenName]);
        var result = await handler.HandleAsync(command, ct);

        if (result.IsFailure)
        {
            if (result.Error.ErrorType == ErrorType.Unauthorized)
                authCookies.ClearTokens(response);

            return result.Error.ToProblem();
        }

        authCookies.SetTokens(response, result.Value.AccessToken, result.Value.RefreshToken);

        return TypedResults.NoContent();
    }
}