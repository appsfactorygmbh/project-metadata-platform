using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to logs in the Project Metadata Platform API.
/// </summary>
public class LogsExceptionHandler : ControllerBase, IExceptionHandler<LogException>
{
    /// <summary>
    /// Handles a specific log exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The project exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the log exception.</returns>
    public IActionResult Handle(LogException exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
        return new ObjectResult(new ErrorResponse(exception.Message))
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}
