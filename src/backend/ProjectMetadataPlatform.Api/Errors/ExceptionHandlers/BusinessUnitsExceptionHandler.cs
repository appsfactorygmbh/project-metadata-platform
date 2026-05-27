using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to bu's in the Project Metadata Platform API.
/// </summary>
public class BusinessUnitsExceptionHandler
    : ControllerBase,
        IExceptionHandler<BusinessUnitException>
{
    /// <summary>
    /// Handles a specific bu exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The bu exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the bu exception.</returns>
    public IActionResult? Handle(BusinessUnitException exception)
    {
        return exception switch
        {
            BusinessUnitStillLinkedToTeamsException businessUnitStillLinkedToTeamsException =>
                BadRequest(new ErrorResponse(businessUnitStillLinkedToTeamsException.Message)),
            _ => null,
        };
    }
}
