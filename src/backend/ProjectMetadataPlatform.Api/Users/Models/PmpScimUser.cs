using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Represents the request to create a new user.
/// </summary>
/// <param name="Email">Email address of a user</param>
/// <param name="Password">Password of a user</param>
public record PmpScimUser
{
    public string[]? Schemas { get; set; } =
    [
        "urn:ietf:params:scim:schemas:core:2.0:User",
        "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
    ];
    public string? Id { get; set; }

    public required string ExternalId { get; set; }

    public required string UserName { get; set; }

    public bool? Active { get; set; }

    public string? Password { get; set; }

    [JsonPropertyName("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
    public EnterpriseUserExtension? EnterpriseUser { get; set; }

    [JsonPropertyName("urn:ietf:params:scim:schemas:extension:pmp:User")]
    public PmpUserExtension? PmpUser { get; set; }

    public MetaResourceData Meta { get; set; } = new MetaResourceData();

    public record EnterpriseUserExtension
    {
        public string? Organization { get; set; }
    }

    public record PmpUserExtension
    {
        public List<string>? Departments { get; set; }

        public List<string>? TeamSupport { get; set; }

        public List<string>? JobTitles { get; set; }

        public List<string>? Teams { get; set; }
        public List<string>? BusinessUnits { get; set; }
    }

    public record MetaResourceData
    {
        public string? ResourceType { get; set; }

        public string? Created { get; set; }

        public string? LastModified { get; set; }

        public string? Location { get; set; }

        public string? Version { get; set; }
    }
}
