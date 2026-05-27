using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to office locations in the Project Metadata Platform API.
/// </summary>
public class OfficeLocationsExceptionHandler
    : ControllerBase,
        IExceptionHandler<OfficeLocationException>
{
    /// <summary>
    /// Handles a specific office location exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The office location exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the office location exception.</returns>
    public IActionResult? Handle(OfficeLocationException exception)
    {
        return exception switch
        {
            _ => null,
        };
    }
}
