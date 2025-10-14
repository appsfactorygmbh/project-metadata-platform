using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to authorization in the Project Metadata Platform API.
/// </summary>
public class AuthorizationExceptionHandler
    : ControllerBase,
        IExceptionHandler<AuthorizationException>
{
    /// <summary>
    /// Handles a specific authorization exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The project exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the log exception.</returns>
    public IActionResult? Handle(AuthorizationException exception)
    {
        return new UnauthorizedResult();
    }
}
