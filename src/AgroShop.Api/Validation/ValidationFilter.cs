using System.Text.Json;
using AgroShop.Api.ErrorHandling;
using FluentValidation;

namespace AgroShop.Api.Validation;

internal sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        TRequest request = context.Arguments.OfType<TRequest>().FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Endpoint has no argument of type {typeof(TRequest).Name} to validate.");

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (result.IsValid)
            return await next(context);

        ApiError[] errors = result.Errors
            .Select(failure => new ApiError(ToJsonName(failure.PropertyName), failure.ErrorCode, failure.ErrorMessage))
            .ToArray();

        return TypedResults.Problem(ApiProblemDetails.Create(
            StatusCodes.Status422UnprocessableEntity, "Validation failed", errors));
    }

    private static string ToJsonName(string propertyPath)
        => string.Join('.', propertyPath.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
}