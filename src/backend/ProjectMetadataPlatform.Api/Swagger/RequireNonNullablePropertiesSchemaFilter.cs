using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectMetadataPlatform.Api.Swagger;

/// <summary>
/// Makes all non-nullable properties required in the open api schema.
/// </summary>
/// <remarks>
/// This is necessary because Swagger does not automatically detect nullable properties.
/// </remarks>
public class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Add to schema.Required all properties where Nullable is false.
    /// </summary>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {

        if (schema is not OpenApiSchema openApiSchema)
            return;

        if (openApiSchema.Properties is not { Count: > 0 })
            return;

        openApiSchema.Required ??= new HashSet<string>();

        var additionalRequiredProps = openApiSchema
            .Properties.Where(x =>
                !IsPropertyNullable(x, context)
                && !openApiSchema.Required.Contains(x.Key)
                && !IsCreatedResponse(x)
            )
            .Select(x => x.Key)
            .ToList();

        foreach (var propKey in additionalRequiredProps)
        {
            openApiSchema.Required.Add(propKey);
        }
    }

    /// <summary>
    /// Check if the response is a created response.
    /// </summary>
    public static bool IsCreatedResponse(KeyValuePair<string, IOpenApiSchema> x)
    {
        return x.Key == "201";
    }

    /// <summary>
    /// Determines if a property is nullable by inspecting C# reflection first,
    /// avoiding the need to mutate OpenApiSchemaReference objects.
    /// </summary>
    private static bool IsPropertyNullable(
        KeyValuePair<string, IOpenApiSchema> property,
        SchemaFilterContext context
    )
    {
        var field = context
            .Type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(x =>
                string.Equals(x.Name, property.Key, StringComparison.InvariantCultureIgnoreCase)
            );

        if (field != null)
        {
            var fieldType = field switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => null,
            };

            if (fieldType != null)
            {
                return fieldType.IsValueType
                    ? Nullable.GetUnderlyingType(fieldType) != null
                    : !field.IsNonNullableReferenceType();
            }
        }

        return property.Value.Type.HasValue
            && property.Value.Type.Value.HasFlag(JsonSchemaType.Null);
    }
}
