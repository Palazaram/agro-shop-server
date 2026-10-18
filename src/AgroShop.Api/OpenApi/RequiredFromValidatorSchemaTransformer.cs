using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AgroShop.Api.OpenApi;

internal sealed class RequiredFromValidatorSchemaTransformer(
    IServiceScopeFactory scopeFactory,
    ILogger<RequiredFromValidatorSchemaTransformer> logger) : IOpenApiSchemaTransformer
{
    private readonly ConcurrentDictionary<Type, HashSet<string>?> requiredByType = new();

    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (schema.Properties is not { Count: > 0 } properties)
            return Task.CompletedTask;

        HashSet<string>? requiredProperties =
            requiredByType.GetOrAdd(context.JsonTypeInfo.Type, InferRequiredProperties);

        if (requiredProperties is null)
            return Task.CompletedTask;

        HashSet<string> required = properties.Keys
            .Where(requiredProperties.Contains)
            .ToHashSet(StringComparer.Ordinal);

        schema.Required = required.Count > 0 ? required : null;

        foreach (string propertyName in required)
        {
            if (properties[propertyName] is OpenApiSchema propertySchema
                && propertySchema.Type is { } propertyType
                && propertyType != JsonSchemaType.Null)
            {
                propertySchema.Type = propertyType & ~JsonSchemaType.Null;
            }
        }

        return Task.CompletedTask;
    }

    private HashSet<string>? InferRequiredProperties(Type contractType)
    {
        try
        {
            using IServiceScope scope = scopeFactory.CreateScope();

            if (scope.ServiceProvider.GetService(typeof(IValidator<>).MakeGenericType(contractType))
                    is not IValidator validator)
                return null;

            object emptyContract = RuntimeHelpers.GetUninitializedObject(contractType);

            ValidationResult result = validator.Validate(new ValidationContext<object>(emptyContract));

            return result.Errors
                .Select(failure => failure.PropertyName)
                .Where(name => !string.IsNullOrEmpty(name) && !name.Contains('.') && !name.Contains('['))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception,
                "Could not infer required properties of {Contract} from its validator", contractType.Name);

            return null;
        }
    }
}