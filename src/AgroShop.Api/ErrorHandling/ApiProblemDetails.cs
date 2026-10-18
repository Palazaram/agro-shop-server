using Microsoft.AspNetCore.Mvc;

namespace AgroShop.Api.ErrorHandling;

internal static class ApiProblemDetails
{
    public static ProblemDetails Create(int status, string title, params ApiError[] errors)
    {
        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title
        };

        problemDetails.Extensions["errors"] = errors;

        return problemDetails;
    }
}