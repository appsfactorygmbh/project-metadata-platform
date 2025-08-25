using MediatR;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Command to create a new user.
/// </summary>
/// <param name="Email">Email of the user.</param>
/// <param name="Password">Password of the user.</param>
public record CreateUserCommand(string Email, string Password) : IRequest<string>;
