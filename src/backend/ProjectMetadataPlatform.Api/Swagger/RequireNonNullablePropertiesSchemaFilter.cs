using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
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
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        FixNullableProperties(schema, context);
        var additionalRequiredProps = schema
            .Properties.Where(x =>
                !x.Value.Nullable && !schema.Required.Contains(x.Key) && !IsCreatedResponse(x)
            )
            .Select(x => x.Key)
            .ToList();
        foreach (var propKey in additionalRequiredProps)
        {
            schema.Required.Add(propKey);
        }
    }

    /// <summary>
    /// Check if the response is a created response.
    /// </summary>
    public static bool IsCreatedResponse(KeyValuePair<string, OpenApiSchema> x)
    {
        return x.Key == "201";
    }

    /// <summary>
    /// Copy + Paste from here: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2036
    /// Option "SupportNonNullableReferenceTypes" not working with complex types ({ "type": "object" }),
    /// so they always have "Nullable = false",
    /// see method "SchemaGenerator.GenerateSchemaForMember"
    /// </summary>
    private static void FixNullableProperties(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var property in schema.Properties)
        {
            if (property.Value.Reference != null)
            {
                var field = context
                    .Type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x =>
                        string.Equals(
                            x.Name,
                            property.Key,
                            StringComparison.InvariantCultureIgnoreCase
                        )
                    );

                if (field != null)
                {
                    var fieldType = field switch
                    {
                        FieldInfo fieldInfo => fieldInfo.FieldType,
                        PropertyInfo propertyInfo => propertyInfo.PropertyType,
                        _ => throw new NotSupportedException(),
                    };

                    property.Value.Nullable = fieldType.IsValueType
                        ? Nullable.GetUnderlyingType(fieldType) != null
                        : !field.IsNonNullableReferenceType();
                }
            }
        }
    }
}
