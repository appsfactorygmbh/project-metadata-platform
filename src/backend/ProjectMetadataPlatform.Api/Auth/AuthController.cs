using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Domain.Auth;

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

    /// <summary>
    /// Returns a List of all Api Tokens without details.
    /// </summary>
    /// <returns>List of Api Tokens.</returns>
    /// <response code="200">Returns the Api Tokens.</response>
    [HttpGet("ApiTokens")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
    [ProducesResponseType(typeof(IEnumerable<GetApiTokenResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<GetApiTokenResponse>>> GetApiTokens()
    {
        var query = new GetAllApiTokensQuery();
        var tokens = await _mediator.Send(query);
        var response = tokens.Select(t => new GetApiTokenResponse(t.Id, t.Name));
        return Ok(response);
    }

    /// <summary>
    /// Gets the Api Token with the given id.
    /// </summary>
    /// <param name="tokenId">Id of the requested token.</param>
    /// <returns>The Api Token with its details minus the actual token value.</returns>
    /// <response code="200">Returns the Api Token.</response>
    /// <response code="404">If the token wasn't found.</response>
    [HttpGet("ApiTokens/{tokenId}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetApiTokenDetailsResponse>> GetApiToken(int tokenId)
    {
        var command = new GetApiTokenDetailsQuery(tokenId);
        var token = await _mediator.Send(command);
        var response = new GetApiTokenDetailsResponse(
            token.Id,
            token.Name,
            token.Scopes ?? [],
            token.ExpirationDate
        );
        return Ok(response);
    }

    /// <summary>
    /// Creates a new token.
    /// </summary>
    /// <param name="request">Request to create the token.</param>
    /// <returns>The details of the token with its actual value.</returns>
    /// <response code="201">If the token was created succesfully.</response>
    /// <response code="400">If the token could not be created.</response>
    [HttpPost("ApiTokens")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
    [ProducesResponseType(typeof(GetApiTokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GetApiTokenDetailsResponse>> PostApiToken(
        [FromBody] CreateApiTokenRequest request
    )
    {
        var command = new CreateApiTokenCommand(request.Name, request.Scopes);
        var token = await _mediator.Send(command);
        var response = new GetApiTokenDetailsResponse(
            token.Id,
            token.Name,
            token.Scopes ?? [],
            token.ExpirationDate,
            token.Token
        );
        var uri = "/ApiTokens/" + token.Id;
        return Created(uri, response);
    }

    /// <summary>
    /// Generates a new Token value for an existing token.
    /// </summary>
    /// <param name="tokenId">Id of the token that should be regenerated.</param>
    /// <returns>The Api Token details with a new value and expiration date.</returns>
    /// <response code="200">Returns the Api Token with the new value.</response>
    /// <response code="404">If the token wasn't found.</response>
    [HttpPatch("ApiTokens/{tokenId}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetApiTokenDetailsResponse>> RegenerateApiToken(int tokenId)
    {
        var command = new RegenerateApiTokenCommand(tokenId);
        var token = await _mediator.Send(command);
        var response = new GetApiTokenDetailsResponse(
            token.Id,
            token.Name,
            token.Scopes ?? [],
            token.ExpirationDate,
            token.Token
        );
        return Ok(response);
    }

    /// <summary>
    /// Deletes the token with the given id.
    /// </summary>
    /// <param name="tokenId"> Id of the token.</param>
    /// <returns>No Content</returns>
    /// <response code="200">If the token was deleted succesfully.</response>
    /// <response code="404">If the token wasn't found.</response>
    [HttpDelete("ApiTokens/{tokenId}")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.SELECTOR)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteApiToken(int tokenId)
    {
        var command = new DeleteApiTokenCommand(tokenId);
        await _mediator.Send(command);
        return NoContent();
    }
}
