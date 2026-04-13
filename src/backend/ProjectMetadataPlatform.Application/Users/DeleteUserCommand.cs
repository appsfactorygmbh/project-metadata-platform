using MediatR;
using Microsoft.AspNetCore.Identity;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a command to delete a user by their Employee Number.
/// </summary>
/// <param name="EmployeeId">The Employee Number of the user to be deleted.</param>
/// <returns>A task that represents the asynchronous operation. The task result contains the deleted user if the operation was successful, otherwise null.</returns>
public record DeleteUserCommand(string EmployeeId) : IRequest<ApplicationUser>;
