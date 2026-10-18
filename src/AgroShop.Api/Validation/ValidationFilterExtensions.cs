namespace AgroShop.Api.Validation;

internal static class ValidationFilterExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        => builder
            .AddEndpointFilter<ValidationFilter<TRequest>>()
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
}