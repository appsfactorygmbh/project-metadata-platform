using System.Collections.Generic;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a command to patch user information.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Email">The new email address of the user, or null to leave unchanged.</param>
/// <param name="Password">The new password of the user, or null to leave unchanged.</param>
public record PatchUserCommand : IRequest<ApplicationUser>
{
    public string Id { get; init; }
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
