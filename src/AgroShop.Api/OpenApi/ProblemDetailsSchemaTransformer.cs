using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace AgroShop.Api.OpenApi;

internal sealed class ProblemDetailsSchemaTransformer : IOpenApiSchemaTransformer
{
    private static readonly string[] AlwaysPresent =
        ["title", "status", "instance", "errors", "traceId", "timestamp"];

    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.JsonTypeInfo.Type != typeof(ProblemDetails))
            return Task.CompletedTask;

        schema.Properties ??= new Dictionary<string, IOpenApiSchema>();

        schema.Properties["errors"] = new OpenApiSchema
        {
            Type = JsonSchemaType.Array,
            Items = CreateApiErrorSchema()
        };

        schema.Properties["traceId"] = new OpenApiSchema { Type = JsonSchemaType.String };
        schema.Properties["timestamp"] = new OpenApiSchema { Type = JsonSchemaType.String, Format = "date-time" };

        schema.Required = new HashSet<string>(AlwaysPresent);

        foreach (string name in AlwaysPresent)
        {
            if (schema.Properties[name] is OpenApiSchema property && property.Type is { } type)
                property.Type = type & ~JsonSchemaType.Null;
        }

        return Task.CompletedTask;
    }

    private static OpenApiSchema CreateApiErrorSchema() => new()
    {
        Type = JsonSchemaType.Object,
        Required = new HashSet<string> { "field", "code", "message" },
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["field"] = new OpenApiSchema { Type = JsonSchemaType.String | JsonSchemaType.Null },
            ["code"] = new OpenApiSchema { Type = JsonSchemaType.String },
            ["message"] = new OpenApiSchema { Type = JsonSchemaType.String }
        }
    };
}