using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to projects in the Project Metadata Platform API.
/// </summary>
public class TeamExceptionHandler : ControllerBase, IExceptionHandler<TeamException>
{
    /// <summary>
    /// Handles a specific team exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The team exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the team exception.</returns>
    public IActionResult? Handle(TeamException exception)
    {
        return exception switch
        {
            TeamStillLinkedToProjectsException teamStillLinkedException => BadRequest(
                new ErrorResponse(teamStillLinkedException.Message)
            ),
            _ => null,
        };
    }
}
