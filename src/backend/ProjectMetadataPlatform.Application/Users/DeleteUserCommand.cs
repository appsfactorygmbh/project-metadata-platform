using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a command to delete a user by their unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the user to be deleted.</param>
/// <returns>A task that represents the asynchronous operation. The task result contains the deleted user if the operation was successful, otherwise null.</returns>
public record DeleteUserCommand(string Id) : IRequest<IdentityUser>;
