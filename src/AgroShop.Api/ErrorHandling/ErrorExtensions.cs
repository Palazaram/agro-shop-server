using AgroShop.Domain.Errors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AgroShop.Api.ErrorHandling;

internal static class ErrorExtensions
{
    public static ProblemHttpResult ToProblem(this Error error)
    {
        var (status, title) = error.ErrorType switch
        {
            ErrorType.Validation => (StatusCodes.Status422UnprocessableEntity, "Validation failed"),
            ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not found"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            _ => throw new ArgumentOutOfRangeException(
                nameof(error), error.ErrorType, "No HTTP status is mapped for this error type.")
        };

        return TypedResults.Problem(
            ApiProblemDetails.Create(status, title, new ApiError(null, error.Code, error.Message)));
    }
}