using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to departments in the Project Metadata Platform API.
/// </summary>
public class DepartmentsExceptionHandler : ControllerBase, IExceptionHandler<DepartmentException>
{
    /// <summary>
    /// Handles a specific department exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The department exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the department exception.</returns>
    public IActionResult? Handle(DepartmentException exception)
    {
        return exception switch
        {
            _ => null,
        };
    }
}
