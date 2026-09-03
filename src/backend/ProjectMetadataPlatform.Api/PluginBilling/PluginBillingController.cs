using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.PluginBilling.Models;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.PluginBilling;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.PluginBilling;

/// <summary>
/// Endpoints for managing Plugin Billing.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("/Projects")]
public class PluginBillingController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="PluginBillingController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public PluginBillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adds new billing information to a project plugin.
    /// </summary>
    /// <param name="request">Billing Creation request.</param>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>Id of the new billing object.</returns>
    /// <response code="201"> Object succesfully created.</response>
    /// <response code="400"> Bad Request.</response>
    /// <response code="404"> Object wasnt found.</response>
    /// <response code="409"> Object Already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut("{projectId:int}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(typeof(AddPluginBillingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddPluginBillingResponse>> AddPluginBilling(
        [FromBody] AddPluginBillingRequest request,
        int projectId,
        int pluginId
    )
    {
        return await ProcessAddPluginBilling(request, projectId, pluginId, projectId.ToString());
    }

    /// <summary>
    /// Adds new billing information to a project plugin.
    /// </summary>
    /// <param name="request">Billing Creation request.</param>
    /// <param name="slug">Slug of the project.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>Id of the new billing object.</returns>
    /// <response code="201"> Object succesfully created.</response>
    /// <response code="400"> Bad Request.</response>
    /// <response code="409"> Object Already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut("{slug}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(typeof(AddPluginBillingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddPluginBillingResponse>> AddPluginBillingBySlug(
        [FromBody] AddPluginBillingRequest request,
        string slug,
        int pluginId
    )
    {
        var projectId = await GetProjectId(slug);
        return await ProcessAddPluginBilling(request, projectId, pluginId, slug);
    }

    /// <summary>
    /// Processes a add Plugin Billing Request
    /// </summary>
    /// <param name="request">The Request.</param>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="pluginId">Id of the plugin</param>
    /// <param name="projectIdentifier">Project Id / slug based on call route.</param>
    /// <returns>Id of the created billing object.</returns>
    private async Task<ActionResult<AddPluginBillingResponse>> ProcessAddPluginBilling(
        AddPluginBillingRequest request,
        int projectId,
        int pluginId,
        string projectIdentifier
    )
    {
        var command = new AddPluginBillingCommand(
            projectId,
            pluginId,
            request.BillingId,
            request.DisplayName,
            request.Currency,
            request.BudgetLimit,
            request.HostingFee,
            request.TargetMargin,
            request.TimeFrame,
            request.Date,
            request.Notes
        );
        (int projectId, int pluginId) ids = await _mediator.Send<
            AddPluginBillingCommand,
            (int, int)
        >(command);

        var response = new AddPluginBillingResponse(ids.projectId, ids.pluginId);
        return Created(
            "/Projects/" + projectIdentifier + "/plugins/" + pluginId + "/billing",
            response
        );
    }

    /// <summary>
    /// Updates Project Plugin Billing Information
    /// </summary>
    /// <param name="request">Update Request.</param>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="pluginId">Id of the project.</param>
    /// <returns>Updated Billing Information.</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{projectId:int}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(typeof(GetPluginBillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPluginBillingResponse>> UpdatePluginBilling(
        [FromBody] UpdatePluginBillingRequest request,
        int projectId,
        int pluginId
    )
    {
        var command = new UpdatePluginBillingCommand(
            projectId,
            pluginId,
            request.DisplayName,
            request.Currency,
            request.BudgetLimit,
            request.HostingFee,
            request.TargetMargin,
            request.TimeFrame,
            request.Date,
            request.Notes
        );
        var billing = await _mediator.Send<
            UpdatePluginBillingCommand,
            Domain.Billing.PluginBilling
        >(command);

        var response = new GetPluginBillingResponse(
            billing.ProjectId,
            billing.PluginId,
            billing.BillingId,
            billing.DisplayName,
            billing.Currency,
            billing.BudgetLimit,
            billing.HostingFee,
            billing.TargetMargin,
            billing.TimeFrame,
            billing.Date,
            billing.Notes
        );
        return Ok(response);
    }

    /// <summary>
    /// Updates Project Plugin Billing Information
    /// </summary>
    /// <param name="request">Update Request.</param>
    /// <param name="slug">Slug of the project.</param>
    /// <param name="pluginId">Id of the project.</param>
    /// <returns>Updated Billing Information.</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{slug}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(typeof(GetPluginBillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPluginBillingResponse>> UpdatePluginBillingBySlug(
        [FromBody] UpdatePluginBillingRequest request,
        string slug,
        int pluginId
    )
    {
        var projectId = await GetProjectId(slug);
        return await UpdatePluginBilling(request, projectId, pluginId);
    }

    /// <summary>
    /// Deletes project plugin billing information.
    /// </summary>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>No Content.</returns>
    /// <response code="204"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{projectId:int}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePluginBilling(int projectId, int pluginId)
    {
        var command = new DeletePluginBillingCommand(projectId, pluginId);
        await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Deletes project plugin billing information.
    /// </summary>
    /// <param name="slug">Slug of the project.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>No Content.</returns>
    /// <response code="204"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{slug}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePluginBillingBySlug(string slug, int pluginId)
    {
        var projectId = await GetProjectId(slug);
        return await DeletePluginBilling(projectId, pluginId);
    }

    /// <summary>
    /// Returns project plugin billing information.
    /// </summary>
    /// <param name="projectId">Id of the project.</param>
    /// <param name="pluginId">Id of the plugin.</param>
    /// <returns>Billing information.</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{projectId:int}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(typeof(GetPluginBillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPluginBillingResponse>> GetPluginBilling(
        int projectId,
        int pluginId
    )
    {
        var query = new GetPluginBillingQuery(projectId, pluginId);
        var (billing, permissions) = await _mediator.Send<
            GetPluginBillingQuery,
            (Domain.Billing.PluginBilling, IEnumerable<AuthorizationConstants.Actions>)
        >(query);
        var response = new GetPluginBillingResponse(
            billing.ProjectId,
            billing.PluginId,
            billing.BillingId,
            billing.DisplayName,
            billing.Currency,
            billing.BudgetLimit,
            billing.HostingFee,
            billing.TargetMargin,
            billing.TimeFrame,
            billing.Date,
            billing.Notes,
            [.. permissions]
        );
        return Ok(response);
    }

    /// <summary>
    /// Returns project plugin billing information.
    /// </summary>
    /// <param name="slug">Slug of the Project.</param>
    /// <param name="pluginId">Id of the project plugin.</param>
    /// <returns>Billing Information.</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{slug}/plugins/{pluginId:int}/billing")]
    [ProducesResponseType(typeof(GetPluginBillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetPluginBillingResponse>> GetPluginBillingBySlug(
        string slug,
        int pluginId
    )
    {
        var projectId = await GetProjectId(slug);
        return await GetPluginBilling(projectId, pluginId);
    }

    /// <summary>
    /// Gets a project id by its slug.
    /// </summary>
    /// <param name="slug">The slug of the project.</param>
    /// <returns>The id of the project.</returns>
    private async Task<int> GetProjectId(string slug)
    {
        var query = new GetProjectIdBySlugQuery(slug);
        return await _mediator.Send<GetProjectIdBySlugQuery, int>(query);
    }
}
