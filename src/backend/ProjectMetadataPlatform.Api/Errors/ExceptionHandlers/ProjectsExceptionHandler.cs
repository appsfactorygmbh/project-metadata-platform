using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to projects in the Project Metadata Platform API.
/// </summary>
public class ProjectsExceptionHandler : ControllerBase, IExceptionHandler<ProjectException>
{
    /// <summary>
    /// Handles a specific project exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The project exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the project exception.</returns>
    public IActionResult? Handle(ProjectException exception)
    {
        return exception switch
        {
            ProjectNotArchivedException projectNotArchivedException => BadRequest(
                new ErrorResponse(projectNotArchivedException.Message)
            ),
            ProjectNotesSizeException projectNotesSizeException => BadRequest(
                new ErrorResponse(projectNotesSizeException.Message)
            ),
            _ => null,
        };
    }
}
