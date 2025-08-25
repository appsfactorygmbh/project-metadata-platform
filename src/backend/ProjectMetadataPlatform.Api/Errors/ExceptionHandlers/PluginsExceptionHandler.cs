using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to plugins in the Project Metadata Platform API.
/// </summary>
public class PluginsExceptionHandler : ControllerBase, IExceptionHandler<PluginException>
{
    /// <summary>
    /// Handles a specific plugin exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The plugin exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the plugin exception.</returns>
    public IActionResult? Handle(PluginException exception)
    {
        return exception switch
        {
            PluginNotArchivedException pluginNotArchivedException => BadRequest(
                new ErrorResponse(pluginNotArchivedException.Message)
            ),
            _ => null,
        };
    }
}
