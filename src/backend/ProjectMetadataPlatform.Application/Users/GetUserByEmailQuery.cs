using MediatR;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Represents a query to get a user by their email.
/// </summary>
/// <param name="Email">The email of the user to retrieve.</param>
/// <returns>The user with the specified email, or null if not found.</returns>
public record GetUserByEmailQuery(string Email) : IRequest<ApplicationUser>;
