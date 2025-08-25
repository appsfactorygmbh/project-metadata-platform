using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles authentication exceptions and returns appropriate HTTP responses.
/// </summary>
public class AuthExceptionHandler : ControllerBase, IExceptionHandler<AuthException>
{
    /// <summary>
    /// Handles the specified authentication exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The authentication exception to handle.</param>
    /// <returns>An IActionResult representing the HTTP response.</returns>
    public IActionResult? Handle(AuthException exception)
    {
        return exception switch
        {
            AuthInvalidLoginCredentialsException authInvalidLoginCredentialsException => BadRequest(
                new ErrorResponse(authInvalidLoginCredentialsException.Message)
            ),
            AuthInvalidRefreshTokenException authInvalidRefreshTokenException => BadRequest(
                new ErrorResponse(authInvalidRefreshTokenException.Message)
            ),
            _ => null,
        };
    }
}
