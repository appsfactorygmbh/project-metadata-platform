using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Auth;

namespace ProjectMetadataPlatform.Api.Auth;

/// <summary>
/// Endpoints for managing authentication.
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Creates a new instance of the <see cref="AuthController"/>.
    /// </summary>
    /// <param name="mediator">The mediator instance for handling requests.</param>
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Logs in a user with the given credentials.
    /// </summary>
    /// <param name="request">The request body containing email and password.</param>
    /// <returns>An <see cref="LoginResponse"/>.</returns>
    /// <response code="200">Returns the access and refresh tokens.</response>
    /// <response code="400">If the credentials are invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost("basic")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest request)
    {
        var query = new LoginQuery(request.Email, request.Password);
        var tokens = await _mediator.Send(query);
        return new LoginResponse(tokens.AccessToken!, tokens.RefreshToken!);
    }

    /// <summary>
    /// Returns a new access token using the given refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh Token header in the format 'Refresh refreshToken'</param>
    /// <returns>An <see cref="LoginResponse"/>.</returns>
    /// <response code="200">Returns the access and refresh tokens.</response>
    /// <response code="400">If the refresh token or the header format are invalid.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponse>> Get(
        [FromHeader(Name = "Authorization")] string refreshToken
    )
    {
        if (!refreshToken.StartsWith("Refresh "))
        {
            return BadRequest(new ErrorResponse("Invalid Header format"));
        }

        var query = new RefreshTokenQuery(refreshToken.Replace("Refresh ", ""));
        var tokens = await _mediator.Send(query);
        return new LoginResponse(tokens.AccessToken!, tokens.RefreshToken!);
    }
}
