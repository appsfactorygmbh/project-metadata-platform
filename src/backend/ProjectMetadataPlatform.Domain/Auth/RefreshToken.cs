using System;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Domain.Auth;

/// <summary>
/// Represents a refresh token.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Id of the refresh token.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Value of the token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// User associated with the token.
    /// </summary>
    public IdentityUser? User { get; set; }

    /// <summary>
    /// Id of the user.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Expiration date of the token.
    /// </summary>
    public DateTimeOffset ExpirationDate { get; set; }
}
