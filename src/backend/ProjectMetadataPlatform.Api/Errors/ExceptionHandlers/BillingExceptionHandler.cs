using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

namespace ProjectMetadataPlatform.Api.Errors.ExceptionHandlers;

/// <summary>
/// Handles exceptions related to billing in the Project Metadata Platform API.
/// </summary>
public class BillingsExceptionHandler : ControllerBase, IExceptionHandler<BillingException>
{
    /// <summary>
    /// Handles a specific billing exception and returns an appropriate HTTP response.
    /// </summary>
    /// <param name="exception">The billing exception to handle.</param>
    /// <returns>An IActionResult representing the result of handling the billing exception.</returns>
    public IActionResult? Handle(BillingException exception)
    {
        return exception switch
        {
            PluginBillingDateMissingException pluginBillingDateMissingException => BadRequest(
                new ErrorResponse(pluginBillingDateMissingException.Message)
            ),
            PluginBillingNotesSizeException pluginBillingNotesSizeException => BadRequest(
                new ErrorResponse(pluginBillingNotesSizeException.Message)
            ),
            _ => null,
        };
    }
}
