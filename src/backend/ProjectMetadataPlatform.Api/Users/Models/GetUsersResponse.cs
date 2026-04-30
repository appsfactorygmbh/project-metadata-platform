using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Response for getting a List of Users. Follow the Scim standard for a List Response.
/// </summary>
public record GetUsersResponse
{
    /// <summary>
    /// Schema of the response.
    /// </summary>
    public string[]? Schemas { get; set; } = ["urn:ietf:params:scim:api:messages:2.0:ListResponse"];

    /// <summary>
    /// Amount of users in the response.
    /// </summary>
    public int TotalResults { get; set; }

    /// <summary>
    /// List of returned users.
    /// </summary>
    [JsonPropertyName("Resources")]
    public IEnumerable<PmpScimUser> Resources { get; init; } = new List<PmpScimUser>();
}
