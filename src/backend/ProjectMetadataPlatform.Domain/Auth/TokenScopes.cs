using System;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Enum representing different scopes for api tokens.
/// </summary>
public enum TokenScopes
{
    /// <summary>
    /// Scope for a Scim Token.
    /// </summary>
    SCIM,

}
