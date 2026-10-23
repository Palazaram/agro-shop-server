using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

namespace AgroShop.Api.ErrorHandling;

internal sealed class BadRequestExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    ILogger<BadRequestExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequest)
            return false;

        logger.LogWarning("Bad request {Method} {Path}: {Reason}",
            httpContext.Request.Method, httpContext.Request.Path, badRequest.Message);

        var problemDetails = ApiProblemDetails.Create(
            badRequest.StatusCode,
            ReasonPhrases.GetReasonPhrase(badRequest.StatusCode),
            new ApiError(null, "request.invalid", "Не вдалося прочитати запит"));

        problemDetails.Detail = environment.IsDevelopment()
            ? badRequest.InnerException?.Message ?? badRequest.Message
            : null;

        httpContext.Response.StatusCode = badRequest.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}