using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Users.Models;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.UserException;

namespace ProjectMetadataPlatform.Api.Users;

/// <summary>
/// Endpoint for user management.
/// </summary>
[ApiController]
[Authorize]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Creates a new instance of the <see cref="UsersController" /> class.
    /// </summary>
    /// <param name="mediator">The mediator instance used for sending commands and queries.</param>
    /// <param name="httpContextAccessor">The http context accessor instance used for accessing the current user.</param>
    public UsersController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="request">Request containing user information</param>
    /// <returns>Statuscode representing the result of user creation</returns>
    /// <response code="201">The user was created successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    /// <response code="400">The request was invalid.</response>
    [HttpPut]
    [ProducesResponseType(typeof(CreateUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateUserResponse>> Put([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ErrorResponse("email and password can't be empty."));
        }

        var command = new CreateUserCommand(request.Email, request.Password);
        var id = await _mediator.Send(command);

        var response = new CreateUserResponse(id);
        var uri = "/Users/" + id;
        return Created(uri, response);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>All users.</returns>
    /// <response code="200">The users are returned successfully.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GetUserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetUserResponse>>> Get()
    {
        var query = new GetAllUsersQuery();
        var users = await _mediator.Send(query);

        var response = users.Select(user => new GetUserResponse(user.Id, user.Email ?? ""));
        return Ok(response);
    }

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve.</param>
    /// <returns>The user with the specified ID.</returns>
    /// <response code="200">The user is returned successfully.</response>
    /// <response code="404">The user with the specified ID was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserResponse>> GetUserById(string userId)
    {
        var query = new GetUserQuery(userId);
        var user = await _mediator.Send(query);

        var response = new GetUserResponse(user.Id, user.Email ?? "");
        return Ok(response);
    }

    /// <summary>
    /// Patches the user information.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to be patched.</param>
    /// <param name="request">The request model containing the new user information.</param>
    /// <returns>The updated user information.</returns>
    /// <response code="200">The user was patched successfully.</response>
    /// <response code="400">The request was invalid.</response>
    /// <response code="404">The user with the specified ID was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [HttpPatch("{userId}")]
    public async Task<ActionResult<GetUserResponse>> Patch(
        string userId,
        [FromBody] PatchUserRequest request
    )
    {
        var command = new PatchUserCommand(userId, request.Email, request.Password);

        var user = await _mediator.Send(command);

        var response = new GetUserResponse(user.Id, user.Email ?? "");
        return Ok(response);
    }

    /// <summary>
    /// Gets the current authenticated user's information.
    /// </summary>
    /// <returns>The current user's information.</returns>
    /// <response code="200">The user information is returned successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The user was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpGet("Me")]
    [ProducesResponseType(typeof(GetUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetUserResponse>> GetMe()
    {
        var email =
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UserUnauthorizedException();

        var query = new GetUserByEmailQuery(email);

        var user = await _mediator.Send(query);

        var response = new GetUserResponse(user.Id, user.Email ?? "");
        return Ok(response);
    }

    /// <summary>
    /// Deletes a user by their userId.
    /// </summary>
    /// <param name="userId">The userId of the user to delete.</param>
    /// <returns>A status code representing the result of the delete operation.</returns>
    /// <response code="204">The user was deleted successfully.</response>
    /// <response code="400">The user tried deleting themselves. The request is invalid.</response>
    /// <response code="404">The user was not found.</response>
    /// <response code="500">An internal error occurred.</response>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(string userId)
    {
        var command = new DeleteUserCommand(userId);

        await _mediator.Send(command);

        return NoContent();
    }
}
