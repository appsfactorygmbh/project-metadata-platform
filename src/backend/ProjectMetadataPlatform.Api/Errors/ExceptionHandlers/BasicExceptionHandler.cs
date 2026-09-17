using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors;
using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions that are not use case specific for the Project Metadata Platform API.
/// </summary>
public partial class BasicExceptionHandler : ControllerBase, IExceptionHandler<PmpException>
{
    private readonly ILogger<BasicExceptionHandler> _logger;

    /// <summary>
    /// Constructor for <see cref="BasicExceptionHandler"/>
    /// </summary>
    /// <param name="logger"></param>
    public BasicExceptionHandler(ILogger<BasicExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles the specified basic exception and returns an appropriate IActionResult.
    /// </summary>
    /// <param name="exception">The basic exception to Handle.</param>
    /// <returns>An IActionResult representing the result of handling the exception.</returns>
    public IActionResult? Handle(PmpException exception)
    {
        return exception switch
        {
            EntityNotFoundException entityNotFoundException => NotFound(
                new ErrorResponse(entityNotFoundException.Message)
            ),
            EntityAlreadyExistsException entityAlreadyExistsException => Conflict(
                new ErrorResponse(entityAlreadyExistsException.Message)
            ),
            DatabaseException databaseException => HandleDatabaseException(databaseException),
            _ => null,
        };
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An error occurred while accessing the database."
    )]
    private static partial void LogDatabaseError(ILogger logger, Exception exception);

    /// <summary>
    /// Handles database exceptions and returns a 502 status.
    /// </summary>
    /// <param name="databaseException">The database exception to handle.</param>
    /// <returns>A StatusCodeResult representing the result of handling the database exception.</returns>
    private ObjectResult HandleDatabaseException(DatabaseException databaseException)
    {
        LogDatabaseError(_logger, databaseException);
        return StatusCode(
            StatusCodes.Status502BadGateway,
            new ErrorResponse("An error occurred while accessing the database.")
        );
    }
}
