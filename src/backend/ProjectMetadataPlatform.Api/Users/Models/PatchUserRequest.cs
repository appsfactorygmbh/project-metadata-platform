using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Api.Users.Models;

/// <summary>
/// Represents a request model for patching user information. Follows Scim Patch schema.
/// </summary>
public record PatchUserRequest
{
    /// <summary>
    /// Schema of the Request.
    /// </summary>
    public string[]? Schemas { get; set; } = ["urn:ietf:params:scim:api:messages:2.0:PatchOp"];

    /// <summary>
    /// List of requested Update operations.
    /// </summary>
    [JsonPropertyName("Operations")]
    public List<OperationRecord> Operations { get; init; } = [];

    /// <summary>
    /// Record representing one Update Operation.
    /// </summary>
    public record OperationRecord
    {
        /// <summary>
        /// Update Operation.
        /// </summary>
        public required PatchOperations Op { get; init; }

        /// <summary>
        /// Path to the Attribute that should be updated.
        /// </summary>
        public required string Path { get; init; }

        /// <summary>
        /// New Value for the Attribute. Ignored for Remove Operations.
        /// </summary>
        public JsonElement? Value { get; init; }
    }
};
