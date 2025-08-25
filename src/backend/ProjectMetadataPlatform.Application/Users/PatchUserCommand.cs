using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a command to patch user information.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Email">The new email address of the user, or null to leave unchanged.</param>
/// <param name="Password">The new password of the user, or null to leave unchanged.</param>
public record PatchUserCommand(string Id, string? Email = null, string? Password = null)
    : IRequest<IdentityUser>;
