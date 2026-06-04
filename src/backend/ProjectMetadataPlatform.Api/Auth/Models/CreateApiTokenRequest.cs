using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.Auth.Models;

/// <summary>
/// Request for creating a new Api Token.
/// </summary>
/// <param name="Name">Name of the Token.</param>
/// <param name="Scopes">Scopes of the Token.</param>
public record CreateApiTokenRequest(string Name, List<TokenScopes> Scopes);
