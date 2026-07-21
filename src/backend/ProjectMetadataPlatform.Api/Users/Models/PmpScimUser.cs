using System.Collections.Generic;
using System.Text.Json.Serialization;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Represents a User. Follows the Scim standard for a user object with a custom pmp extension.
/// </summary>
public record PmpScimUser
{
    /// <summary>
    /// Schemas of the user object.
    /// </summary>
    public string[]? Schemas { get; set; } =
    [
        "urn:ietf:params:scim:schemas:core:2.0:User",
        "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
        "urn:ietf:params:scim:schemas:extension:pmp:User",
    ];

    /// <summary>
    /// Id of the user. In our case the employee Number.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// External Id of the user. In our case the employee Number.
    /// </summary>
    public required string ExternalId { get; set; }

    /// <summary>
    /// Username of the user. In our case the email.
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    /// Wether the user is an active employee.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Password of the user. Not managed by entra.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// List of addresses of the User.
    /// </summary>
    [JsonPropertyName("addresses")]
    public List<AddressRecord>? Addresses { get; set; } = [];

    /// <summary>
    /// Contains properties of the enterprise extension.
    /// </summary>
    [JsonPropertyName("urn:ietf:params:scim:schemas:extension:enterprise:2.0:User")]
    public EnterpriseUserExtension? EnterpriseUser { get; set; }

    /// <summary>
    /// Contains properties of the pmp extension.
    /// </summary>
    [JsonPropertyName("urn:ietf:params:scim:schemas:extension:pmp:User")]
    public PmpUserExtension? PmpUser { get; set; }

    /// <summary>
    /// Contains Metadata.
    /// </summary>
    public MetaResourceData Meta { get; set; } = new MetaResourceData();

    /// <summary>
    /// Record representing an Address of a User.
    /// </summary>
    public record AddressRecord
    {
        /// <summary>
        /// City or region of the address.
        /// </summary>
        [JsonPropertyName("locality")]
        public string? Locality { get; set; }
    }

    /// <summary>
    /// Record representing properties of a Enterprise User.
    /// </summary>
    public record EnterpriseUserExtension
    {
        /// <summary>
        /// Organization the user is a part of. In our case the company name.
        /// </summary>
        public string? Organization { get; set; }
    }

    /// <summary>
    /// Record representing properties of a User from the Appsfactory Group.
    /// </summary>
    public record PmpUserExtension
    {
        /// <summary>
        /// List of departments the user belongs to.
        /// </summary>
        public List<string>? Departments { get; set; }

        /// <summary>
        /// List of teamnames of teams the user is supporting
        /// </summary>
        public List<string>? TeamSupport { get; set; }

        /// <summary>
        /// List of jobtitles of the user.
        /// </summary>
        public List<string>? JobTitles { get; set; }

        /// <summary>
        /// List of teamnames of teams the user belongs to.
        /// </summary>
        public List<string>? Team { get; set; }

        /// <summary>
        /// List of BUs the user belongs to.
        /// </summary>
        public List<string>? BusinessUnits { get; set; }

        /// <summary>
        /// Wether the last change to the user was made via scim or manually.
        /// </summary>
        public bool? IsScimProvisioned { get; set; }
    }

    /// <summary>
    /// Record representing metadata properties.
    /// </summary>
    public record MetaResourceData
    {
        /// <summary>
        /// Type of the object.
        /// </summary>
        public string? ResourceType { get; set; }

        /// <summary>
        /// Creation date for the resource.
        /// </summary>
        public string? Created { get; set; }

        /// <summary>
        /// Date of the last update to the resource.
        /// </summary>
        public string? LastModified { get; set; }

        /// <summary>
        /// Uri to the resource.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Version of the resource.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Permissions on the User resource.
        /// </summary>
        public List<AuthorizationConstants.Actions>? Permissions { get; set; }
    }
}
