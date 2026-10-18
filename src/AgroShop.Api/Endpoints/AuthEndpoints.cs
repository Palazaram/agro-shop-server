using AgroShop.Api.Authentication;
using AgroShop.Api.ErrorHandling;
using AgroShop.Api.Validation;
using AgroShop.Application.Abstractions;
using AgroShop.Application.Users.RegisterUser;
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
}