using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Command for creating an Api Token.
/// </summary>
/// <param name="Name">Name of the token.</param>
/// <param name="Scopes">List of Token scopes.</param>
public record CreateApiTokenCommand(string Name, List<TokenScopes> Scopes) : IRequest<ApiToken>;
