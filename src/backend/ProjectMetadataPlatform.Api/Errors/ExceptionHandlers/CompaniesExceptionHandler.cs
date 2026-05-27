using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to companies in the Project Metadata Platform API.
/// </summary>
public class CompaniesExceptionHandler : ControllerBase, IExceptionHandler<CompanyException>
{
    /// <summary>
    /// Handles a specific department exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The department exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the department exception.</returns>
    public IActionResult? Handle(CompanyException exception)
    {
        return exception switch
        {
            CompanyStillLinkedToProjectsException companyStillLinkedToProjectsException =>
                BadRequest(new ErrorResponse(companyStillLinkedToProjectsException.Message)),
            _ => null,
        };
    }
}
