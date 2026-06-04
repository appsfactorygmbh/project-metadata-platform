using System;
using System.Collections.Generic;

namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Represents an long lived token used for authentication.
/// </summary>
public class ApiToken
{
    /// <summary>
    /// Id of the api token.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Value of the token.
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Name of the token.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// List of token scopes.
    /// </summary>
    public List<TokenScopes>? Scopes { get; set; }

    /// <summary>
    /// Expiration date of the token.
    /// </summary>
    public DateTimeOffset ExpirationDate { get; set; }
}
