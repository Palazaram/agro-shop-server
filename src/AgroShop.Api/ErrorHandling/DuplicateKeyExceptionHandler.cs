using AgroShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AgroShop.Api.ErrorHandling;

internal sealed class DuplicateKeyExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DuplicateKeyExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DuplicateKeyException duplicateKey)
            return false;

        logger.LogWarning("Unique constraint {Constraint} was violated", duplicateKey.ConstraintName);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict"
        };

        problemDetails.Extensions["errors"] = new[]
        {
            new ApiError(null, "resource.duplicate", "A record with the same unique value already exists.")
        };

        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}