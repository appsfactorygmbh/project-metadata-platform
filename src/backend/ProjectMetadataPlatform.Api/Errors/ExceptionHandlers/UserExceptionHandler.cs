using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions of type <see cref="UserException"/>.
/// </summary>
public class UserExceptionHandler : ControllerBase, IExceptionHandler<UserException>
{
    /// <summary>
    /// Handles the specified <see cref="UserException"/>.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the exception handling.</returns>
    public IActionResult? Handle(UserException exception)
    {
        return exception switch
        {
            UserInvalidPasswordFormatException => BadRequest(new ErrorResponse(exception.Message)),
            UserUnauthorizedException => Unauthorized(new ErrorResponse(exception.Message)),
            UserCantDeleteThemselfException => BadRequest(new ErrorResponse(exception.Message)),
            UserCouldNotBeDeletedException => BadRequest(new ErrorResponse(exception.Message)),
            UserCouldNotBeCreatedException => BadRequest(new ErrorResponse(exception.Message)),
            _ => null,
        };
    }
}
