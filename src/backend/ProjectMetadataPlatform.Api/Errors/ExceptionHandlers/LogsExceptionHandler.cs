using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to logs in the Project Metadata Platform API.
/// </summary>
public partial class LogsExceptionHandler : ControllerBase, IExceptionHandler<LogException>
{
    private readonly ILogger<LogsExceptionHandler> _logger;

    /// <summary>
    /// Constructor for <see cref="LogsExceptionHandler"/>
    /// </summary>
    /// <param name="logger"></param>
    public LogsExceptionHandler(ILogger<LogsExceptionHandler> logger)
    {
        _logger = logger;
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An error occurred while accessing the database."
    )]
    private static partial void LogLogError(ILogger logger, Exception exception);

    /// <summary>
    /// Handles a specific log exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The project exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the log exception.</returns>
    public IActionResult Handle(LogException exception)
    {
        LogLogError(_logger, exception);
        return new ObjectResult(new ErrorResponse(exception.Message))
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };
    }
}
