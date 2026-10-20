using System.Security.Claims;
using AgroShop.Api.Authentication;
using AgroShop.Api.ErrorHandling;
using AgroShop.Application.Users;
using AgroShop.Application.Users.GetCurrentUser;
using AgroShop.Domain.Errors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AgroShop.Api.Endpoints;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization();

        group.MapGet("/me", GetCurrentUserAsync)
            .WithName("GetCurrentUser")
            .WithSummary("Get the profile of the signed-in user")
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<Results<Ok<UserProfileResponse>, ProblemHttpResult>> GetCurrentUserAsync(
        ClaimsPrincipal user,
        IUserQueries userQueries,
        CancellationToken ct)
    {
        var profile = await userQueries.GetProfileAsync(user.GetUserId(), ct);

        if (profile.HasNoValue)
            return DomainErrors.Users.NotFound().ToProblem();

        return TypedResults.Ok(profile.Value);
    }
}