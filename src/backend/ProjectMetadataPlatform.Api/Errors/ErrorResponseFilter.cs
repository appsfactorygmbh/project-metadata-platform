using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectMetadataPlatform.Api.Errors;

/// <summary>
/// Filter to handle error responses.
/// </summary>
public sealed class ErrorResponseFilter : IAlwaysRunResultFilter
{
    /// <inheritdoc />
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (
            context.Result is ObjectResult
            {
                Value: string message,
                StatusCode: >= 400 and var statusCode
            }
        )
        {
            context.Result = new ObjectResult(new ErrorResponse(message))
            {
                StatusCode = statusCode,
            };
        }
    }

    /// <inheritdoc />
    public void OnResultExecuted(ResultExecutedContext context) { }
}
