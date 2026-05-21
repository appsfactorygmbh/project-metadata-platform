using MediatR;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Command to delete a token.
/// </summary>
/// <param name="TokenId">Id of the token.</param>
public record DeleteApiTokenCommand(int TokenId) : IRequest;
