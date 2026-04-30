using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a command to patch user information.
/// </summary>
public record PatchUserCommand : IRequest<ApplicationUser>
{
    /// <summary>
    /// Employee Id of the user.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// List of update operations.
    /// </summary>
    public List<OperationRecord> Operations { get; init; } = new List<OperationRecord>();

    /// <summary>
    /// Record representing one Update Operation.
    /// </summary>
    public record OperationRecord
    {
        /// <summary>
        /// Update Operation.
        /// </summary>
        public required PatchOperations Operation { get; init; }

        /// <summary>
        /// Path to the Attribute that should be updated.
        /// </summary>
        public required string Path { get; init; }

        /// <summary>
        /// New Value for the Attribute. Ignored for Remove Operations.
        /// </summary>
        public object? Value { get; init; }
    }
};
