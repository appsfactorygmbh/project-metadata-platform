using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectMetadataPlatform.Api.Swagger;

/// <summary>
/// Adjusts the description of the Unauthorized response.
/// </summary>
public class UnauthorizedResponseOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (
            context.ApiDescription.RelativePath?.StartsWith(
                "auth",
                StringComparison.InvariantCultureIgnoreCase
            )
            is true
        )
        {
            operation.Responses.Remove("401");
            return;
        }

        if (operation.Responses.TryGetValue("401", out var value))
        {
            value.Description =
                "The user is not logged in or does not have the necessary permissions to perform this action.";
        }
    }
}
