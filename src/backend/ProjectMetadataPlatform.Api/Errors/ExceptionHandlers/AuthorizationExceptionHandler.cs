using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

public class AuthorizationExceptionHandler
    : ControllerBase,
        IExceptionHandler<AuthorizationException>
{
    public IActionResult? Handle(AuthorizationException exception)
    {
        return new UnauthorizedResult();
    }
}
