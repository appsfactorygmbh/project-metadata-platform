using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Billing.Models;
using ProjectMetadataPlatform.Api.Common.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Billing;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Api.Billing;

/// <summary>
/// Endpoints for managing billing.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
[Route("[controller]")]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="BillingController" />.
    /// </summary>
    /// <param name="mediator"></param>
    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new global billing object.
    /// </summary>
    /// <param name="request">Creation Request.</param>
    /// <returns>Id of the new billing information. </returns>
    /// <response code="201"> Object succesfully created.</response>
    /// <response code="400"> Bad Request.</response>
    /// <response code="409"> Object Already exists.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CreateBillingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBillingResponse>> Put(
        [FromBody] CreateBillingRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.BillingKind))
        {
            return BadRequest(new ErrorResponse("BillingKind can't be empty or whitespaces"));
        }

        var command = new CreateBillingCommand(
            request.BillingKind,
            request.Currency,
            request.BudgetLimit,
            request.HostingFee,
            request.TargetMargin,
            request.TimeFrame
        );
        var billingId = await _mediator.Send<CreateBillingCommand, int>(command);
        var response = new CreateBillingResponse(billingId);
        var uri = "/Billing/" + billingId;
        return Created(uri, response);
    }

    /// <summary>
    /// Updates existing global billing information.
    /// </summary>
    /// <param name="billingId">Id of the billing object.</param>
    /// <param name="request">Update Request.</param>
    /// <returns>Updated billing information.</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="400"> Bad Request.</response>
    /// <response code="409"> Object Already exists.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpPatch("{billingId:int}")]
    [ProducesResponseType(typeof(GetBillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateBillingResponse>> Update(
        int billingId,
        [FromBody] UpdateBillingRequest request
    )
    {
        if (string.IsNullOrWhiteSpace(request.BillingKind))
        {
            return BadRequest(new ErrorResponse("BillingKind can't be empty or whitespaces"));
        }

        var command = new UpdateBillingCommand(
            billingId,
            request.BillingKind,
            request.Currency,
            request.BudgetLimit,
            request.HostingFee,
            request.TargetMargin,
            request.TimeFrame
        );
        var billing = await _mediator.Send<UpdateBillingCommand, GlobalBilling>(command);
        var response = new GetBillingResponse(
            billingId,
            billing.BillingKind,
            billing.Currency,
            billing.BudgetLimit,
            billing.HostingFee,
            billing.TargetMargin,
            billing.TimeFrame
        );

        return Ok(response);
    }

    /// <summary>
    /// Return billing information by id.
    /// </summary>
    /// <param name="billingId">Id of the billing information.</param>
    /// <returns>The Billing Information</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{billingId:int}")]
    [ProducesResponseType(typeof(GetBillingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetBillingResponse>> Get(int billingId)
    {
        var query = new GetBillingByIdQuery(billingId);
        var (billing, permissions) = await _mediator.Send<
            GetBillingByIdQuery,
            (GlobalBilling, IEnumerable<AuthorizationConstants.Actions>)
        >(query);

        var billingResponse = new GetBillingResponse(
            billing.Id,
            billing.BillingKind,
            billing.Currency,
            billing.BudgetLimit,
            billing.HostingFee,
            billing.TargetMargin,
            billing.TimeFrame,
            [.. permissions]
        );

        return Ok(billingResponse);
    }

    /// <summary>
    /// Return all global billing information.
    /// </summary>
    /// <returns>List of global billing information.</returns>
    /// <response code="200"> Request Successfull.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetListResponse<GetBillingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetListResponse<GetBillingResponse>>> Get()
    {
        var query = new GetAllBillingQuery();
        var (billing, globalPermissions) = await _mediator.Send<
            GetAllBillingQuery,
            (IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>)
        >(query);

        var billingResponse = billing.Select(billing => new GetBillingResponse(
            billing.Id,
            billing.BillingKind,
            billing.Currency,
            billing.BudgetLimit,
            billing.HostingFee,
            billing.TargetMargin,
            billing.TimeFrame
        ));
        var response = new GetListResponse<GetBillingResponse>(
            [.. billingResponse],
            [.. globalPermissions]
        );
        return Ok(response);
    }

    /// <summary>
    /// Deletes global Billing information.
    /// </summary>
    /// <param name="id">Id of the billing object.</param>
    /// <returns>No Content Result.</returns>
    /// <response code="204"> Request Successfull.</response>
    /// <response code="404"> Object not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteBillingCommand(id);

        await _mediator.Send(command);

        return NoContent();
    }
}
