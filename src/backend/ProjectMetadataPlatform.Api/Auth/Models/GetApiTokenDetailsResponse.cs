using System;
using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Auth.Models;

/// <summary>
/// Response representing a Api Token with its details.
/// </summary>
/// <param name="Id">Internal identifier of the token.</param>
/// <param name="Name">Name of the token.</param>
/// <param name="Scopes">Scopes of the token.</param>
/// <param name="ExpirationDate">Expiration date of the token.</param>
/// <param name="Token">The token value. Should only be set when responding to token creation or regeneration.</param>
/// <param name="Permissions">Permissions on the Resource</param>
public record GetApiTokenDetailsResponse(
    int Id,
    string Name,
    List<TokenScopes> Scopes,
    DateTimeOffset ExpirationDate,
    string? Token = null,
    List<AuthorizationConstants.Actions>? Permissions = null
);
